using FluentValidation;
using FarmNet.Client.Models;

namespace FarmNet.Client.Validators;

public class TaoNhatKyRequestValidator : AbstractValidator<TaoNhatKyRequest>
{
    public TaoNhatKyRequestValidator()
    {
        RuleFor(x => x.HoatDong)
            .NotEmpty().WithMessage("Vui lòng nhập hoạt động")
            .MaximumLength(200).WithMessage("Hoạt động tối đa 200 ký tự");

        RuleFor(x => x.NgayThucHien)
            .NotEmpty().WithMessage("Vui lòng chọn ngày thực hiện");

        RuleFor(x => x.GhiChu)
            .MaximumLength(500).WithMessage("Ghi chú tối đa 500 ký tự");
    }
}
