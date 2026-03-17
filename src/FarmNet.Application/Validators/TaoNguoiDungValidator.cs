using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class TaoNguoiDungValidator : AbstractValidator<TaoNguoiDungRequest>
{
    private static readonly string[] VaiTroHopLe = ["Admin", "FarmOwner", "Worker"];

    public TaoNguoiDungValidator()
    {
        RuleFor(x => x.HoTen).NotEmpty().MaximumLength(100).WithMessage("Họ tên không được để trống (tối đa 100 ký tự)");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email không hợp lệ");
        RuleFor(x => x.MatKhau).NotEmpty().MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");
        RuleFor(x => x.VaiTro).Must(v => VaiTroHopLe.Contains(v)).WithMessage("Vai trò không hợp lệ");
    }
}
