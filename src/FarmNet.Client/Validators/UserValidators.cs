using FluentValidation;
using FarmNet.Client.Models;

namespace FarmNet.Client.Validators;

public class TaoNguoiDungRequestValidator : AbstractValidator<TaoNguoiDungRequest>
{
    public TaoNguoiDungRequestValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Vui lòng nhập họ tên")
            .MaximumLength(100).WithMessage("Họ tên tối đa 100 ký tự");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Vui lòng nhập email")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Vui lòng nhập mật khẩu")
            .MinimumLength(6).WithMessage("Mật khẩu tối thiểu 6 ký tự");

        RuleFor(x => x.VaiTro)
            .NotEmpty().WithMessage("Vui lòng chọn vai trò");
    }
}

public class CapNhatNguoiDungRequestValidator : AbstractValidator<CapNhatNguoiDungRequest>
{
    public CapNhatNguoiDungRequestValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Vui lòng nhập họ tên")
            .MaximumLength(100).WithMessage("Họ tên tối đa 100 ký tự");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Vui lòng nhập email")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.MatKhauMoi)
            .MinimumLength(6).WithMessage("Mật khẩu tối thiểu 6 ký tự")
            .When(x => !string.IsNullOrEmpty(x.MatKhauMoi));
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Vui lòng nhập email")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Vui lòng nhập mật khẩu");
    }
}
