using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface IFarmingLogService
{
    Task<IEnumerable<FarmingLogDto>> GetByBatchAsync(Guid batchId);
    Task<Result<FarmingLogDto>> CreateAsync(TaoNhatKyRequest request, string userId);
}
