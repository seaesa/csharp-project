using FarmNet.Domain.Enums;

namespace FarmNet.Application.DTOs.Responses;

public record BatchDto(
    Guid Id,
    string MaLo,
    string TenSanPham,
    string? MoTa,
    BatchStatus TrangThai,
    string TenTrangThai,
    DateTime NgayTao,
    Guid FarmId,
    string TenFarm
)
{
    public BatchDto() : this(default, string.Empty, string.Empty, null, default, string.Empty, default, default, string.Empty) { }
}
