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
    IDailyHashService dailyHashService) : IBatchService
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
        var txHash = await blockchain.RecordHashAsync(dataHash, BlockchainEventType.TaoLo, batch.MaLo);

        await blockchainRepo.AddAsync(new BlockchainRecord
        {
            Id = Guid.NewGuid(),
            BatchId = batch.Id,
            EntityId = batch.Id,
            LoaiSuKien = BlockchainEventType.TaoLo,
            DataHash = dataHash,
            TxHash = string.IsNullOrEmpty(txHash) ? null : txHash,
            DaXacNhan = !string.IsNullOrEmpty(txHash)
        });
        await uow.SaveChangesAsync();

        var result = await repo.Query().Include(b => b.Farm).FirstOrDefaultAsync(b => b.Id == batch.Id);
        return Result.Ok(mapper.Map<BatchDto>(result!));
    }

    public async Task<Result<BatchDto>> UpdateAsync(Guid id, TaoBatchRequest request)
    {
        var batch = await repo.GetByIdAsync(id);
        if (batch == null) return Result.Fail("Không tìm thấy lô sản phẩm");

        batch.TenSanPham = request.TenSanPham;
        batch.MoTa = request.MoTa;
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
        batch.TrangThai = status;
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

    public async Task<BlockchainVerifyResultDto?> VerifyBlockchainAsync(Guid batchId) =>
        await dailyHashService.VerifyAsync(batchId);

    private static string TinhHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
