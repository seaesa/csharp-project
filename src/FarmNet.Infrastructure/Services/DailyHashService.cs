using System.Security.Cryptography;
using System.Text;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Enums;
using FarmNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class DailyHashService(
    IRepository<DailyHash> dailyHashRepo,
    IRepository<FarmingLog> farmingLogRepo,
    IRepository<SensorData> sensorDataRepo,
    IRepository<Batch> batchRepo,
    IRepository<BlockchainRecord> blockchainRepo,
    IRepository<Harvest> harvestRepo,
    IUnitOfWork uow,
    IBlockchainService blockchain) : IDailyHashService
{
    public async Task RecalcAsync(Guid batchId, DateOnly date)
    {
        var batch = await batchRepo.GetByIdAsync(batchId);
        if (batch == null) return;

        var (hash, logCount, sensorCount) = await ComputeDailyData(batchId, batch.MaLo, date);

        var existing = await dailyHashRepo.Query()
            .FirstOrDefaultAsync(d => d.BatchId == batchId && d.Ngay == date);

        if (existing != null)
        {
            var hasTx = !string.IsNullOrEmpty(existing.TxHash);

            existing.DataHash = hash;
            existing.SoNhatKy = logCount;
            existing.SoCamBien = sensorCount;
            existing.DaXacNhan = false;
            if (!hasTx)
                existing.TxHash = null;
            dailyHashRepo.Update(existing);
        }
        else
        {
            await dailyHashRepo.AddAsync(new DailyHash
            {
                Id = Guid.NewGuid(),
                BatchId = batchId,
                Ngay = date,
                DataHash = hash,
                SoNhatKy = logCount,
                SoCamBien = sensorCount
            });
        }

        await uow.SaveChangesAsync();
    }

    public async Task RecalcAllAsync(Guid batchId)
    {
        var batch = await batchRepo.GetByIdAsync(batchId);
        if (batch == null) return;

        var dates = await GetAllDatesWithData(batchId, batch.MaLo);
        foreach (var date in dates)
            await RecalcAsync(batchId, date);
    }

    public async Task CommitToBlockchainAsync(Guid batchId)
    {
        await RecalcAllAsync(batchId);

        var uncommitted = await dailyHashRepo.Query()
            .Where(d => d.BatchId == batchId && !d.DaXacNhan)
            .OrderBy(d => d.Ngay)
            .ToListAsync();

        foreach (var dh in uncommitted)
        {
            var txHash = await blockchain.RecordHashAsync(
                dh.DataHash, BlockchainEventType.TongHopHangNgay, batchId.ToString());

            dh.TxHash = string.IsNullOrEmpty(txHash) ? null : txHash;
            dh.DaXacNhan = !string.IsNullOrEmpty(txHash);
            dailyHashRepo.Update(dh);
        }

        await uow.SaveChangesAsync();
    }

    public async Task<BlockchainVerifyResultDto?> VerifyAsync(Guid batchId, int sampleSize = 10)
    {
        var batch = await batchRepo.Query()
            .Include(b => b.Farm)
            .FirstOrDefaultAsync(b => b.Id == batchId);
        if (batch == null) return null;

        var taoLoRecord = await blockchainRepo.Query()
            .FirstOrDefaultAsync(r => r.BatchId == batchId && r.LoaiSuKien == BlockchainEventType.TaoLo);
        var taoLoStatus = await VerifyBookendRecord(taoLoRecord, ComputeTaoLoHash(batch));

        var harvest = await harvestRepo.Query().FirstOrDefaultAsync(h => h.BatchId == batchId);
        var thuHoachRecord = await blockchainRepo.Query()
            .FirstOrDefaultAsync(r => r.BatchId == batchId && r.LoaiSuKien == BlockchainEventType.ThuHoach);
        VerifyStatus? thuHoachStatus = harvest != null
            ? await VerifyBookendRecord(thuHoachRecord, ComputeThuHoachHash(harvest))
            : null;

        var dailyHashes = await dailyHashRepo.Query()
            .Where(d => d.BatchId == batchId)
            .OrderBy(d => d.Ngay)
            .ToListAsync();

        var sampled = SampleDays(dailyHashes, sampleSize, batchId);
        var dailyItems = new List<DailyVerifyItemDto>(sampled.Count);
        foreach (var dh in sampled)
        {
            var (computed, _, _) = await ComputeDailyData(batchId, batch.MaLo, dh.Ngay);
            var status = await VerifyDailyHash(dh, computed);
            dailyItems.Add(new DailyVerifyItemDto(
                dh.Ngay, dh.DataHash, computed,
                status == VerifyStatus.HopLe || status == VerifyStatus.ChuaXacNhan,
                dh.SoNhatKy, dh.SoCamBien, dh.TxHash, status));
        }

        var soHopLe = dailyItems.Count(d => d.TrangThai == VerifyStatus.HopLe);
        var soBiThayDoi = dailyItems.Count(d => d.TrangThai is VerifyStatus.BiThayDoi or VerifyStatus.BlockchainKhongKhop);
        var toanVen = taoLoStatus == VerifyStatus.HopLe
                      && (thuHoachStatus is null or VerifyStatus.HopLe)
                      && soBiThayDoi == 0;

        return new BlockchainVerifyResultDto(
            toanVen, dailyHashes.Count, sampled.Count, soHopLe, soBiThayDoi,
            taoLoStatus, thuHoachStatus, dailyItems);
    }

    public async Task<IEnumerable<DailyHashDto>> GetByBatchAsync(Guid batchId)
    {
        var hashes = await dailyHashRepo.Query()
            .Where(d => d.BatchId == batchId)
            .OrderByDescending(d => d.Ngay)
            .ToListAsync();
        return hashes.Select(d => new DailyHashDto(
            d.Id, d.Ngay, d.DataHash, d.TxHash, d.DaXacNhan,
            d.SoNhatKy, d.SoCamBien, d.NgayTao));
    }

    private async Task<VerifyStatus> VerifyDailyHash(DailyHash dh, string computedHash)
    {
        if (!string.Equals(dh.DataHash, computedHash, StringComparison.OrdinalIgnoreCase))
            return VerifyStatus.BiThayDoi;

        if (string.IsNullOrEmpty(dh.TxHash))
            return VerifyStatus.ChuaXacNhan;

        var fetch = await blockchain.GetRecordedHashAsync(dh.TxHash);
        return fetch.Status switch
        {
            BlockchainFetchStatus.RpcError => VerifyStatus.KhongTruyCapBlockchain,
            BlockchainFetchStatus.TxNotFound => VerifyStatus.ChuaXacNhan,
            BlockchainFetchStatus.Success when !string.Equals(dh.DataHash, fetch.Hash, StringComparison.OrdinalIgnoreCase)
                => VerifyStatus.BlockchainKhongKhop,
            _ => VerifyStatus.HopLe
        };
    }

    private async Task<VerifyStatus> VerifyBookendRecord(BlockchainRecord? record, string expectedHash)
    {
        if (record == null)
            return VerifyStatus.ChuaXacNhan;

        if (!string.Equals(record.DataHash, expectedHash, StringComparison.OrdinalIgnoreCase))
            return VerifyStatus.BiThayDoi;

        if (string.IsNullOrEmpty(record.TxHash))
            return VerifyStatus.ChuaXacNhan;

        var fetch = await blockchain.GetRecordedHashAsync(record.TxHash);
        return fetch.Status switch
        {
            BlockchainFetchStatus.RpcError => VerifyStatus.KhongTruyCapBlockchain,
            BlockchainFetchStatus.TxNotFound => VerifyStatus.ChuaXacNhan,
            BlockchainFetchStatus.Success when !string.Equals(record.DataHash, fetch.Hash, StringComparison.OrdinalIgnoreCase)
                => VerifyStatus.BlockchainKhongKhop,
            _ => VerifyStatus.HopLe
        };
    }

    private async Task<(string Hash, int LogCount, int SensorCount)> ComputeDailyData(
        Guid batchId, string maLo, DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = date.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var logs = await farmingLogRepo.Query()
            .Where(l => l.BatchId == batchId && l.NgayThucHien >= start && l.NgayThucHien < end)
            .OrderBy(l => l.Id)
            .ToListAsync();

        var sensors = await sensorDataRepo.Query()
            .Where(d => d.BatchId == maLo && d.ThoiGian >= start && d.ThoiGian < end)
            .OrderBy(d => d.Id)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.Append($"DAILY|{batchId}|{date:yyyy-MM-dd}");

        foreach (var log in logs)
        {
            var ngay = DateTime.SpecifyKind(log.NgayThucHien, DateTimeKind.Utc);
            sb.Append($"|LOG:{log.Id}:{log.HoatDong}:{ngay:O}:{log.NguoiThucHienId}");
        }

        foreach (var sd in sensors)
        {
            var t = DateTime.SpecifyKind(sd.ThoiGian, DateTimeKind.Utc);
            sb.Append($"|SENSOR:{sd.Id}:{sd.NhietDo}:{sd.DoAm}:{sd.DoPH}:{sd.AnhSang}:{sd.DoAmDat}:{t:O}");
        }

        return (TinhHash(sb.ToString()), logs.Count, sensors.Count);
    }

    private async Task<List<DateOnly>> GetAllDatesWithData(Guid batchId, string maLo)
    {
        var logDates = (await farmingLogRepo.Query()
            .Where(l => l.BatchId == batchId)
            .Select(l => l.NgayThucHien)
            .ToListAsync())
            .Select(d => DateOnly.FromDateTime(DateTime.SpecifyKind(d, DateTimeKind.Utc)));

        var sensorDates = (await sensorDataRepo.Query()
            .Where(d => d.BatchId == maLo)
            .Select(d => d.ThoiGian)
            .ToListAsync())
            .Select(d => DateOnly.FromDateTime(DateTime.SpecifyKind(d, DateTimeKind.Utc)));

        return logDates.Union(sensorDates).Distinct().OrderBy(d => d).ToList();
    }

    private static List<DailyHash> SampleDays(List<DailyHash> all, int size, Guid batchId)
    {
        if (all.Count <= size) return all;
        var rng = new Random(batchId.GetHashCode());
        return [.. all.OrderBy(_ => rng.Next()).Take(size).OrderBy(d => d.Ngay)];
    }

    private static string ComputeTaoLoHash(Batch batch)
    {
        var ngay = DateTime.SpecifyKind(batch.NgayTao, DateTimeKind.Utc);
        return TinhHash($"{batch.Id}|{batch.MaLo}|{batch.FarmId}|{ngay:O}");
    }

    private static string ComputeThuHoachHash(Harvest harvest)
    {
        var ngay = DateTime.SpecifyKind(harvest.NgayThuHoach, DateTimeKind.Utc);
        return TinhHash($"{harvest.BatchId}|{ngay:O}|{harvest.TrongLuong}");
    }

    private static string TinhHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
