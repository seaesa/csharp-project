namespace FarmNet.Application.DTOs.Responses;

public record NguoiDungDto(string Id, string HoTen, string Email, string VaiTro, Guid? FarmId, string? TenFarm);
