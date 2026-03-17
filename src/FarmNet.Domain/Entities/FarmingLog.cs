namespace FarmNet.Domain.Entities;

public class FarmingLog
{
    public Guid Id { get; set; }
    public string HoatDong { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public DateTime NgayThucHien { get; set; } = DateTime.UtcNow;

    public Guid BatchId { get; set; }
    public Batch Batch { get; set; } = null!;

    public string NguoiThucHienId { get; set; } = string.Empty;
    public AppUser NguoiThucHien { get; set; } = null!;
}
