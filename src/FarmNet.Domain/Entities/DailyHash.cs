namespace FarmNet.Domain.Entities;

public class DailyHash
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public DateOnly Ngay { get; set; }
    public string DataHash { get; set; } = string.Empty;
    public string? TxHash { get; set; }
    public bool DaXacNhan { get; set; }
    public int SoNhatKy { get; set; }
    public int SoCamBien { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    public Batch Batch { get; set; } = null!;
}
