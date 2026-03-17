namespace FarmNet.Client.Models;

public class NguoiDungDto
{
    public string Id { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
    public Guid? FarmId { get; set; }
    public string? TenFarm { get; set; }
}

public class TaoNguoiDungRequest
{
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string VaiTro { get; set; } = "Worker";
    public Guid? FarmId { get; set; }
}

public class CapNhatNguoiDungRequest
{
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? FarmId { get; set; }
    public string? MatKhauMoi { get; set; }
}
