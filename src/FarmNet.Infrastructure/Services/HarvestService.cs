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

public class HarvestService(
    IRepository<Harvest> repo,
    IRepository<Batch> batchRepo,
    IRepository<BlockchainRecord> blockchainRepo,
    IUnitOfWork uow,
    IMapper mapper,
    IBlockchainService blockchain,
    IDailyHashService dailyHashService) : IHarvestService
{
    public async Task<HarvestDto?> GetByBatchAsync(Guid batchId)
    {
        var harvest = await repo.Query().FirstOrDefaultAsync(h => h.BatchId == batchId);
        return harvest == null ? null : mapper.Map<HarvestDto>(harvest);
    }

    public async Task<Result<HarvestDto>> CreateAsync(TaoThuHoachRequest request)
    {
        if (await repo.Query().AnyAsync(h => h.BatchId == request.BatchId))
            return Result.Fail("Lô sản phẩm này đã có thông tin thu hoạch");

        var batch = await batchRepo.GetByIdAsync(request.BatchId);
        if (batch == null) return Result.Fail("Không tìm thấy lô sản phẩm");

        var harvest = new Harvest
        {
            Id = Guid.NewGuid(),
            BatchId = request.BatchId,
            NgayThuHoach = request.NgayThuHoach,
            TrongLuong = request.TrongLuong,
            GhiChuChatLuong = request.GhiChuChatLuong
        };

        await repo.AddAsync(harvest);

        batch.TrangThai = BatchStatus.DaThuHoach;
        batch.NgayCapNhat = DateTime.UtcNow;
        batchRepo.Update(batch);
        await uow.SaveChangesAsync();

        var ngayUtc = DateTime.SpecifyKind(harvest.NgayThuHoach, DateTimeKind.Utc);
        var dataHash = TinhHash($"{harvest.BatchId}|{ngayUtc:O}|{harvest.TrongLuong}");
        var txHash = await blockchain.RecordHashAsync(dataHash, BlockchainEventType.ThuHoach, batch.MaLo);

        await blockchainRepo.AddAsync(new BlockchainRecord
        {
            Id = Guid.NewGuid(),
            BatchId = request.BatchId,
            EntityId = harvest.Id,
            LoaiSuKien = BlockchainEventType.ThuHoach,
            DataHash = dataHash,
            TxHash = string.IsNullOrEmpty(txHash) ? null : txHash,
            DaXacNhan = !string.IsNullOrEmpty(txHash)
        });
        await uow.SaveChangesAsync();

        await dailyHashService.CommitToBlockchainAsync(request.BatchId);

        return Result.Ok(mapper.Map<HarvestDto>(harvest));
    }

    private static string TinhHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
