using FluentValidation;
using FarmNet.Client.Models;

namespace FarmNet.Client.Validators;

public class TaoBatchRequestValidator : AbstractValidator<TaoBatchRequest>
{
    public TaoBatchRequestValidator()
    {
        RuleFor(x => x.MaLo)
            .NotEmpty().WithMessage("Vui lòng nhập mã lô")
            .MaximumLength(50).WithMessage("Mã lô tối đa 50 ký tự");

        RuleFor(x => x.TenSanPham)
            .NotEmpty().WithMessage("Vui lòng nhập tên sản phẩm")
            .MaximumLength(100).WithMessage("Tên sản phẩm tối đa 100 ký tự");

        RuleFor(x => x.FarmId)
            .NotEqual(Guid.Empty).WithMessage("Vui lòng chọn trang trại");

        RuleFor(x => x.MoTa)
            .MaximumLength(500).WithMessage("Mô tả tối đa 500 ký tự");
    }
}
