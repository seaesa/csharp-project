using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class TaoFarmValidator : AbstractValidator<TaoFarmRequest>
{
    public TaoFarmValidator()
    {
        RuleFor(x => x.Ten).NotEmpty().MaximumLength(200).WithMessage("Tên trang trại không được để trống");
        RuleFor(x => x.DiaChi).NotEmpty().MaximumLength(500).WithMessage("Địa chỉ không được để trống");
        RuleFor(x => x.MoTa).MaximumLength(1000).When(x => x.MoTa != null);
    }
}
