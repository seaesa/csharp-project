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

public class FarmingLogService(
    IRepository<FarmingLog> repo,
    IRepository<BlockchainRecord> blockchainRepo,
    IUnitOfWork uow,
    IMapper mapper,
    IBlockchainService blockchain) : IFarmingLogService
{
    public async Task<IEnumerable<FarmingLogDto>> GetByBatchAsync(Guid batchId)
    {
        var logs = await repo.Query()
            .Include(l => l.NguoiThucHien)
            .Where(l => l.BatchId == batchId)
            .OrderByDescending(l => l.NgayThucHien)
            .ToListAsync();
        return mapper.Map<IEnumerable<FarmingLogDto>>(logs);
    }

    public async Task<Result<FarmingLogDto>> CreateAsync(TaoNhatKyRequest request, string userId)
    {
        var log = new FarmingLog
        {
            Id = Guid.NewGuid(),
            BatchId = request.BatchId,
            HoatDong = request.HoatDong,
            GhiChu = request.GhiChu,
            NgayThucHien = request.NgayThucHien,
            NguoiThucHienId = userId
        };

        await repo.AddAsync(log);
        await uow.SaveChangesAsync();

        var ngayUtc  = DateTime.SpecifyKind(log.NgayThucHien, DateTimeKind.Utc);
        var dataHash = TinhHash($"{log.BatchId}|{log.HoatDong}|{ngayUtc:O}|{userId}");
        var txHash   = await blockchain.RecordHashAsync(dataHash, BlockchainEventType.NhatKyCanhTac, request.BatchId.ToString());

        var record = new BlockchainRecord
        {
            Id         = Guid.NewGuid(),
            BatchId    = request.BatchId,
            EntityId   = log.Id,                            // 1-to-1 mapping for verify
            LoaiSuKien = BlockchainEventType.NhatKyCanhTac,
            DataHash   = dataHash,
            TxHash     = string.IsNullOrEmpty(txHash) ? null : txHash,
            DaXacNhan  = !string.IsNullOrEmpty(txHash)
        };
        await blockchainRepo.AddAsync(record);
        await uow.SaveChangesAsync();

        var result = await repo.Query().Include(l => l.NguoiThucHien).FirstOrDefaultAsync(l => l.Id == log.Id);
        return Result.Ok(mapper.Map<FarmingLogDto>(result!));
    }

    private static string TinhHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
