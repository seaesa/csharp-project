using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Domain.Enums;

namespace FarmNet.Application.Services;

public interface IBatchService
{
    Task<IEnumerable<BatchDto>> GetAllAsync(Guid? farmId = null);
    Task<BatchDto?> GetByIdAsync(Guid id);
    Task<Result<BatchDto>> CreateAsync(TaoBatchRequest request);
    Task<Result<BatchDto>> UpdateAsync(Guid id, TaoBatchRequest request);
    Task<Result> UpdateStatusAsync(Guid id, BatchStatus status);
    Task<IEnumerable<BlockchainRecordDto>> GetBlockchainRecordsAsync(Guid batchId);
    Task<BlockchainVerifyResultDto?> VerifyBlockchainAsync(Guid batchId);
}
