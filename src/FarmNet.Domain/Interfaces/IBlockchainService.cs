using FarmNet.Domain.Entities;
using FarmNet.Domain.Enums;

namespace FarmNet.Domain.Interfaces;

public interface IBlockchainService
{
    Task<string> RecordHashAsync(string dataHash, BlockchainEventType eventType, string batchId);
    Task<bool> VerifyHashAsync(string txHash, string dataHash);
}
