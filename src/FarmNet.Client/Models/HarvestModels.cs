namespace FarmNet.Client.Models;

public class HarvestDto
{
    public Guid Id { get; set; }
    public DateTime NgayThuHoach { get; set; }
    public double TrongLuong { get; set; }
    public string? GhiChuChatLuong { get; set; }
    public Guid BatchId { get; set; }
}

public class TaoThuHoachRequest
{
    public Guid BatchId { get; set; }
    public DateTime NgayThuHoach { get; set; } = DateTime.Now;
    public double TrongLuong { get; set; }
    public string? GhiChuChatLuong { get; set; }
}
