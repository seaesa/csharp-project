namespace FarmNet.Application.DTOs.Responses;

public record AuthResponse(string Token, string Email, string HoTen, string VaiTro, DateTime HetHan);
