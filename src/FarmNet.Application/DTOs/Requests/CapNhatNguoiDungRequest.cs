namespace FarmNet.Application.DTOs.Requests;

public record CapNhatNguoiDungRequest(
    string HoTen,
    string Email,
    Guid? FarmId,
    string? MatKhauMoi
);
