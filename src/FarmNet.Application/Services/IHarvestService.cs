using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface IHarvestService
{
    Task<HarvestDto?> GetByBatchAsync(Guid batchId);
    Task<Result<HarvestDto>> CreateAsync(TaoThuHoachRequest request);
}
