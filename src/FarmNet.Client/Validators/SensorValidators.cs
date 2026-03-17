using FluentValidation;
using FarmNet.Client.Models;

namespace FarmNet.Client.Validators;

public class TaoSensorRequestValidator : AbstractValidator<TaoSensorRequest>
{
    public TaoSensorRequestValidator()
    {
        RuleFor(x => x.Ten)
            .NotEmpty().WithMessage("Vui lòng nhập tên cảm biến")
            .MaximumLength(100).WithMessage("Tên tối đa 100 ký tự");

        RuleFor(x => x.BatchId)
            .NotEqual(Guid.Empty).WithMessage("Vui lòng chọn lô sản phẩm");
    }
}
