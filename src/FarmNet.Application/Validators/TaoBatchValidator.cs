using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class TaoBatchValidator : AbstractValidator<TaoBatchRequest>
{
    public TaoBatchValidator()
    {
        RuleFor(x => x.MaLo).NotEmpty().MaximumLength(50).WithMessage("Mã lô không được để trống (tối đa 50 ký tự)");
        RuleFor(x => x.TenSanPham).NotEmpty().MaximumLength(200).WithMessage("Tên sản phẩm không được để trống");
        RuleFor(x => x.FarmId).NotEmpty().WithMessage("FarmId không được để trống");
    }
}
