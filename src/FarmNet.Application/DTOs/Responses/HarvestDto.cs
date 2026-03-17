namespace FarmNet.Application.DTOs.Responses;

public record HarvestDto(
    Guid Id,
    DateTime NgayThuHoach,
    double TrongLuong,
    string? GhiChuChatLuong,
    Guid BatchId
)
{
    public HarvestDto() : this(default, default, default, null, default) { }
}
