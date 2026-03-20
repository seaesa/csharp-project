using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface IDailyHashService
{
    Task RecalcAsync(Guid batchId, DateOnly date);
    Task RecalcAllAsync(Guid batchId);
    Task CommitToBlockchainAsync(Guid batchId);
    Task<BlockchainVerifyResultDto?> VerifyAsync(Guid batchId, int sampleSize = 10);
    Task<IEnumerable<DailyHashDto>> GetByBatchAsync(Guid batchId);
}
