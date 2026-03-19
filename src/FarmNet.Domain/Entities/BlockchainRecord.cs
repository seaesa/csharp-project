using FarmNet.Domain.Enums;

namespace FarmNet.Domain.Entities;

public class BlockchainRecord
{
    public Guid Id { get; set; }
    public BlockchainEventType LoaiSuKien { get; set; }
    public Guid? EntityId { get; set; }
    public string DataHash { get; set; } = string.Empty;
    public string? TxHash { get; set; }
    public bool DaXacNhan { get; set; }
    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;

    public Guid BatchId { get; set; }
    public Batch Batch { get; set; } = null!;
}
