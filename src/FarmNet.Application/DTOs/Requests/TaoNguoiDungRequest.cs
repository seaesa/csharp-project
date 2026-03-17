namespace FarmNet.Application.DTOs.Requests;

public record TaoNguoiDungRequest(
    string HoTen,
    string Email,
    string MatKhau,
    string VaiTro,
    Guid? FarmId
);
