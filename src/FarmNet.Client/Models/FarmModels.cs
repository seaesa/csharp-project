namespace FarmNet.Client.Models;

public class FarmDto
{
    public Guid Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayTao { get; set; }
    public int SoLuongLo { get; set; }
}

public class TaoFarmRequest
{
    public string Ten { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string? MoTa { get; set; }
}
