namespace FarmNet.Domain.Entities;

public class Harvest
{
    public Guid Id { get; set; }
    public DateTime NgayThuHoach { get; set; }
    public double TrongLuong { get; set; }
    public string? GhiChuChatLuong { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    public Guid BatchId { get; set; }
    public Batch Batch { get; set; } = null!;
}
