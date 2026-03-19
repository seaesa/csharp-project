using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Enums;
using FarmNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class BatchService(
    IRepository<Batch> repo,
    IUnitOfWork uow,
    IMapper mapper,
    IBlockchainService blockchain,
    IRepository<BlockchainRecord> blockchainRepo,
    IRepository<FarmingLog> farmingLogRepo,
    IRepository<Harvest> harvestRepo) : IBatchService
{
    public async Task<IEnumerable<BatchDto>> GetAllAsync(Guid? farmId = null)
    {
        var query = repo.Query().Include(b => b.Farm).AsQueryable();
        if (farmId.HasValue) query = query.Where(b => b.FarmId == farmId.Value);
        return mapper.Map<IEnumerable<BatchDto>>(
            await query.OrderByDescending(b => b.NgayTao).ToListAsync());
    }

    public async Task<BatchDto?> GetByIdAsync(Guid id)
    {
        var batch = await repo.Query().Include(b => b.Farm).FirstOrDefaultAsync(b => b.Id == id);
        return batch == null ? null : mapper.Map<BatchDto>(batch);
    }

    public async Task<Result<BatchDto>> CreateAsync(TaoBatchRequest request)
    {
        if (await repo.Query().AnyAsync(b => b.MaLo == request.MaLo))
            return Result.Fail($"Mã lô '{request.MaLo}' đã tồn tại");

        var batch = new Batch
        {
            Id = Guid.NewGuid(),
            MaLo = request.MaLo,
            TenSanPham = request.TenSanPham,
            MoTa = request.MoTa,
            FarmId = request.FarmId
        };

        await repo.AddAsync(batch);
        await uow.SaveChangesAsync();

        var dataHash = TinhHash($"{batch.Id}|{batch.MaLo}|{batch.FarmId}|{batch.NgayTao:O}");
        var txHash   = await blockchain.RecordHashAsync(dataHash, BlockchainEventType.TaoLo, batch.MaLo);

        await blockchainRepo.AddAsync(new BlockchainRecord
        {
            Id         = Guid.NewGuid(),
            BatchId    = batch.Id,
            EntityId   = batch.Id,
            LoaiSuKien = BlockchainEventType.TaoLo,
            DataHash   = dataHash,
            TxHash     = string.IsNullOrEmpty(txHash) ? null : txHash,
            DaXacNhan  = !string.IsNullOrEmpty(txHash)
        });
        await uow.SaveChangesAsync();

        var result = await repo.Query().Include(b => b.Farm).FirstOrDefaultAsync(b => b.Id == batch.Id);
        return Result.Ok(mapper.Map<BatchDto>(result!));
    }

    public async Task<Result<BatchDto>> UpdateAsync(Guid id, TaoBatchRequest request)
    {
        var batch = await repo.GetByIdAsync(id);
        if (batch == null) return Result.Fail("Không tìm thấy lô sản phẩm");

        batch.TenSanPham  = request.TenSanPham;
        batch.MoTa        = request.MoTa;
        batch.NgayCapNhat = DateTime.UtcNow;
        repo.Update(batch);
        await uow.SaveChangesAsync();

        var result = await repo.Query().Include(b => b.Farm).FirstOrDefaultAsync(b => b.Id == id);
        return Result.Ok(mapper.Map<BatchDto>(result!));
    }

    public async Task<Result> UpdateStatusAsync(Guid id, BatchStatus status)
    {
        var batch = await repo.GetByIdAsync(id);
        if (batch == null) return Result.Fail("Không tìm thấy lô sản phẩm");
        batch.TrangThai   = status;
        batch.NgayCapNhat = DateTime.UtcNow;
        repo.Update(batch);
        await uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<IEnumerable<BlockchainRecordDto>> GetBlockchainRecordsAsync(Guid batchId)
    {
        var records = await blockchainRepo.Query()
            .Where(r => r.BatchId == batchId)
            .OrderBy(r => r.ThoiGian)
            .ToListAsync();
        return mapper.Map<IEnumerable<BlockchainRecordDto>>(records);
    }

    public async Task<BlockchainVerifyResultDto?> VerifyBlockchainAsync(Guid batchId)
    {
        var batch = await repo.Query()
            .Include(b => b.Farm)
            .FirstOrDefaultAsync(b => b.Id == batchId);
        if (batch == null) return null;

        var records = await blockchainRepo.Query()
            .Where(r => r.BatchId == batchId)
            .OrderBy(r => r.ThoiGian)
            .ToListAsync();

        if (records.Count == 0)
            return new BlockchainVerifyResultDto(true, 0, 0, 0, 0, []);

        var logs    = await farmingLogRepo.Query().Where(l => l.BatchId == batchId).ToListAsync();
        var harvest = await harvestRepo.Query().FirstOrDefaultAsync(h => h.BatchId == batchId);

        var items = new List<BlockchainVerifyItemDto>(records.Count);
        foreach (var record in records)
            items.Add(await VerifyOneRecordAsync(record, batch, logs, harvest));

        return BuildResult(items);
    }

    private async Task<BlockchainVerifyItemDto> VerifyOneRecordAsync(
        BlockchainRecord record,
        Batch            batch,
        List<FarmingLog> logs,
        Harvest?         harvest)
    {
        var label = GetLabel(record.LoaiSuKien);

        if (record.LoaiSuKien == BlockchainEventType.TongHopCamBien)
            return MakeItem(record, label, string.Empty, VerifyStatus.KhongHoTroLoaiSuKien);

        var hr = ComputeExpectedHash(record, batch, logs, harvest);
        if (!hr.Found)
            return MakeItem(record, label, string.Empty, VerifyStatus.KhongTimThayDuLieu);

        var computedHash = hr.Hash!;

        if (!HashesEqual(record.DataHash, computedHash))
            return MakeItem(record, label, computedHash, VerifyStatus.DbBiThayDoi);

        if (string.IsNullOrEmpty(record.TxHash))
            return MakeItem(record, label, computedHash, VerifyStatus.ChuaXacNhanBlockchain);

        var fetch = await blockchain.GetRecordedHashAsync(record.TxHash);

        return fetch.Status switch
        {
            BlockchainFetchStatus.RpcError   => MakeItem(record, label, computedHash, VerifyStatus.KhongTruyCapBlockchain),
            BlockchainFetchStatus.TxNotFound => MakeItem(record, label, computedHash, VerifyStatus.ChuaXacNhanBlockchain),
            BlockchainFetchStatus.Success when !HashesEqual(record.DataHash, fetch.Hash)
                                             => MakeItem(record, label, computedHash, VerifyStatus.BlockchainKhongKhop),
            _                                => MakeItem(record, label, computedHash, VerifyStatus.HopLe)
        };
    }

    private sealed class HashResult
    {
        public bool    Found { get; private init; }
        public string? Hash  { get; private init; }

        public static HashResult Ok(string hash) => new() { Found = true, Hash = hash };
        public static readonly HashResult Missing = new() { Found = false };
    }

    private static HashResult ComputeExpectedHash(
        BlockchainRecord record,
        Batch            batch,
        List<FarmingLog> logs,
        Harvest?         harvest) =>
        record.LoaiSuKien switch
        {
            BlockchainEventType.TaoLo =>
                HashResult.Ok(ComputeTaoLoHash(batch)),

            BlockchainEventType.NhatKyCanhTac =>
                record.EntityId.HasValue
                    ? FindAndHashFarmingLog(record.EntityId.Value, logs)
                    : HashResult.Missing,

            BlockchainEventType.ThuHoach =>
                harvest != null
                    ? HashResult.Ok(ComputeThuHoachHash(harvest))
                    : HashResult.Missing,

            _ => HashResult.Missing
        };

    private static HashResult FindAndHashFarmingLog(Guid entityId, List<FarmingLog> logs)
    {
        var log = logs.FirstOrDefault(l => l.Id == entityId);
        return log is null ? HashResult.Missing : HashResult.Ok(ComputeFarmingLogHash(log));
    }

    private static string ComputeTaoLoHash(Batch batch)
    {
        var ngayUtc = DateTime.SpecifyKind(batch.NgayTao, DateTimeKind.Utc);
        return TinhHash($"{batch.Id}|{batch.MaLo}|{batch.FarmId}|{ngayUtc:O}");
    }

    private static string ComputeFarmingLogHash(FarmingLog log)
    {
        var ngayUtc = DateTime.SpecifyKind(log.NgayThucHien, DateTimeKind.Utc);
        return TinhHash($"{log.BatchId}|{log.HoatDong}|{ngayUtc:O}|{log.NguoiThucHienId}");
    }

    private static string ComputeThuHoachHash(Harvest harvest)
    {
        var ngayUtc = DateTime.SpecifyKind(harvest.NgayThuHoach, DateTimeKind.Utc);
        return TinhHash($"{harvest.BatchId}|{ngayUtc:O}|{harvest.TrongLuong}");
    }

    private static string TinhHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }

    private static BlockchainVerifyItemDto MakeItem(
        BlockchainRecord record,
        string           label,
        string           computedHash,
        VerifyStatus     status) =>
        new(record.Id, record.LoaiSuKien, label,
            record.DataHash, computedHash, IsIntact(status),
            record.DaXacNhan, record.TxHash, status, record.ThoiGian);

    private static bool IsIntact(VerifyStatus status) =>
        status is VerifyStatus.HopLe
               or VerifyStatus.ChuaXacNhanBlockchain
               or VerifyStatus.KhongTruyCapBlockchain
               or VerifyStatus.KhongHoTroLoaiSuKien;

    private static BlockchainVerifyResultDto BuildResult(List<BlockchainVerifyItemDto> items)
    {
        var hopLe       = items.Count(i => i.TrangThaiXacThuc == VerifyStatus.HopLe);
        var biThayDoi   = items.Count(i => i.TrangThaiXacThuc is
                              VerifyStatus.DbBiThayDoi or VerifyStatus.BlockchainKhongKhop or VerifyStatus.KhongTimThayDuLieu);
        var chuaXacNhan = items.Count(i => i.TrangThaiXacThuc is
                              VerifyStatus.ChuaXacNhanBlockchain or VerifyStatus.KhongTruyCapBlockchain);

        return new BlockchainVerifyResultDto(
            ToanVen: biThayDoi == 0, TongSoBanGhi: items.Count,
            SoBanGhiHopLe: hopLe, SoBanGhiBiThayDoi: biThayDoi,
            SoBanGhiChuaXacNhan: chuaXacNhan, ChiTiet: items);
    }

    private static bool HashesEqual(string a, string? b) =>
        !string.IsNullOrEmpty(b) && string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static string GetLabel(BlockchainEventType type) => type switch
    {
        BlockchainEventType.TaoLo          => "Tạo lô sản phẩm",
        BlockchainEventType.NhatKyCanhTac  => "Nhật ký canh tác",
        BlockchainEventType.TongHopCamBien => "Tổng hợp cảm biến",
        BlockchainEventType.ThuHoach       => "Thu hoạch",
        _                                  => "Không xác định"
    };
}
