using System.Text;
using FarmNet.Domain.Enums;
using FarmNet.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace FarmNet.Infrastructure.Services;

public class BlockchainService : IBlockchainService
{
    private const string TxPrefix      = "FarmNet";
    private const int    TxDataHashIdx = 3;
    private const int    TxMinParts    = 5;

    private readonly Web3?    _web3;
    private readonly Account? _account;
    private readonly ILogger<BlockchainService> _logger;
    private readonly bool _enabled;

    public BlockchainService(IConfiguration config, ILogger<BlockchainService> logger)
    {
        _logger = logger;
        var privateKey = config["Blockchain:PrivateKey"];
        var rpcUrl     = config["Blockchain:RpcUrl"];

        if (string.IsNullOrWhiteSpace(privateKey) || privateKey.StartsWith("YOUR_") ||
            string.IsNullOrWhiteSpace(rpcUrl)     || rpcUrl.Contains("YOUR_"))
        {
            _logger.LogWarning("Blockchain chưa được cấu hình — nghiệp vụ blockchain bị bỏ qua.");
            _enabled = false;
            return;
        }

        try
        {
            _account = new Account(privateKey);
            _web3    = new Web3(_account, rpcUrl);
            _enabled = true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cấu hình blockchain không hợp lệ — nghiệp vụ blockchain bị bỏ qua.");
            _enabled = false;
        }
    }

    public async Task<string> RecordHashAsync(string dataHash, BlockchainEventType eventType, string batchId)
    {
        if (!_enabled) return string.Empty;

        try
        {
            var data    = $"{TxPrefix}|{eventType}|{batchId}|{dataHash}|{DateTime.UtcNow:O}";
            var hexData = "0x" + Convert.ToHexString(Encoding.UTF8.GetBytes(data));

            var txInput = new Nethereum.RPC.Eth.DTOs.TransactionInput
            {
                From = _account!.Address,
                To   = _account.Address,
                Data = hexData,
                Gas  = new Nethereum.Hex.HexTypes.HexBigInteger(21000 + data.Length * 68)
            };

            var txHash = await _web3!.Eth.TransactionManager.SendTransactionAsync(txInput);
            _logger.LogInformation("Blockchain TX ghi thành công: {TxHash}", txHash);
            return txHash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi ghi blockchain cho batch {BatchId}", batchId);
            return string.Empty;
        }
    }

    public async Task<BlockchainFetchResult> GetRecordedHashAsync(string txHash)
    {
        if (!_enabled) return BlockchainFetchResult.Failed;

        try
        {
            var tx = await _web3!.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);

            if (tx?.Input == null)
                return BlockchainFetchResult.NotFound;

            var hex   = tx.Input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? tx.Input[2..] : tx.Input;
            var plain = Encoding.UTF8.GetString(Convert.FromHexString(hex));
            var parts = plain.Split('|');

            if (parts.Length < TxMinParts || parts[0] != TxPrefix)
                return BlockchainFetchResult.NotFound;

            return BlockchainFetchResult.Succeeded(parts[TxDataHashIdx]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi RPC khi đọc TX {TxHash}", txHash);
            return BlockchainFetchResult.Failed;
        }
    }
}
