using FarmNet.Domain.Enums;

namespace FarmNet.Domain.Interfaces;

public interface IBlockchainService
{
    Task<string> RecordHashAsync(string dataHash, BlockchainEventType eventType, string batchId);
    Task<BlockchainFetchResult> GetRecordedHashAsync(string txHash);
}
