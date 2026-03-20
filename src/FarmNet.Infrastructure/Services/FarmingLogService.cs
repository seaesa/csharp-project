using AutoMapper;
using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class FarmingLogService(
    IRepository<FarmingLog> repo,
    IUnitOfWork uow,
    IMapper mapper,
    IDailyHashService dailyHashService) : IFarmingLogService
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

        var ngayUtc = DateTime.SpecifyKind(log.NgayThucHien, DateTimeKind.Utc);
        await dailyHashService.RecalcAsync(request.BatchId, DateOnly.FromDateTime(ngayUtc));

        var result = await repo.Query().Include(l => l.NguoiThucHien).FirstOrDefaultAsync(l => l.Id == log.Id);
        return Result.Ok(mapper.Map<FarmingLogDto>(result!));
    }
}
