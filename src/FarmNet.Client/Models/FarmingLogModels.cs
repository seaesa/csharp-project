namespace FarmNet.Client.Models;

public class FarmingLogDto
{
    public Guid Id { get; set; }
    public string HoatDong { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public DateTime NgayThucHien { get; set; }
    public Guid BatchId { get; set; }
    public string TenNguoiThucHien { get; set; } = string.Empty;
}

public class TaoNhatKyRequest
{
    public Guid BatchId { get; set; }
    public string HoatDong { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public DateTime NgayThucHien { get; set; } = DateTime.Now;
}
