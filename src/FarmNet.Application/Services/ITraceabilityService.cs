using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface ITraceabilityService
{
    Task<TraceabilityDto?> GetTraceAsync(string batchMaLo);
    Task<bool> VerifyBatchAsync(Guid batchId);
}
