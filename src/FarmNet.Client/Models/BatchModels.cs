namespace FarmNet.Client.Models;

public class BatchDto
{
    public Guid Id { get; set; }
    public string MaLo { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public int TrangThai { get; set; }
    public string TenTrangThai { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
    public Guid FarmId { get; set; }
    public string TenFarm { get; set; } = string.Empty;
}

public class TaoBatchRequest
{
    public string MaLo { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public Guid FarmId { get; set; }
}

public class CapNhatTrangThaiBatchRequest
{
    public int TrangThai { get; set; }
}
