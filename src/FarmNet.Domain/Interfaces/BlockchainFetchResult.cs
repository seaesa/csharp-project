namespace FarmNet.Domain.Interfaces;

public enum BlockchainFetchStatus { Success, TxNotFound, RpcError }

public sealed record BlockchainFetchResult(BlockchainFetchStatus Status, string? Hash)
{
    public static BlockchainFetchResult Succeeded(string hash) => new(BlockchainFetchStatus.Success, hash);
    public static readonly BlockchainFetchResult NotFound = new(BlockchainFetchStatus.TxNotFound, null);
    public static readonly BlockchainFetchResult Failed   = new(BlockchainFetchStatus.RpcError, null);
}
