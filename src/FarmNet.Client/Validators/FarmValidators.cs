using FluentValidation;
using FarmNet.Client.Models;

namespace FarmNet.Client.Validators;

public class TaoFarmRequestValidator : AbstractValidator<TaoFarmRequest>
{
    public TaoFarmRequestValidator()
    {
        RuleFor(x => x.Ten)
            .NotEmpty().WithMessage("Vui lòng nhập tên trang trại")
            .MaximumLength(100).WithMessage("Tên tối đa 100 ký tự");

        RuleFor(x => x.DiaChi)
            .NotEmpty().WithMessage("Vui lòng nhập địa chỉ")
            .MaximumLength(200).WithMessage("Địa chỉ tối đa 200 ký tự");

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage("Mô tả tối đa 500 ký tự");
    }
}
