using FarmNet.Domain.Enums;

namespace FarmNet.Application.DTOs.Responses;

public record BlockchainRecordDto(
    Guid Id,
    BlockchainEventType LoaiSuKien,
    string TenSuKien,
    string DataHash,
    string? TxHash,
    bool DaXacNhan,
    DateTime ThoiGian
)
{
    public BlockchainRecordDto() : this(default, default, string.Empty, string.Empty, null, default, default) { }
}
