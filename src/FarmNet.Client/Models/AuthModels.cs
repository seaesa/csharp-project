namespace FarmNet.Client.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
    public DateTime HetHan { get; set; }
}
