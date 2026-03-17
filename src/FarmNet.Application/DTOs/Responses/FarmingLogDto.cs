namespace FarmNet.Application.DTOs.Responses;

public record FarmingLogDto(
    Guid Id,
    string HoatDong,
    string? GhiChu,
    DateTime NgayThucHien,
    Guid BatchId,
    string TenNguoiThucHien
)
{
    public FarmingLogDto() : this(default, string.Empty, null, default, default, string.Empty) { }
}
