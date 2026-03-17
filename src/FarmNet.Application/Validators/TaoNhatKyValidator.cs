using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class TaoNhatKyValidator : AbstractValidator<TaoNhatKyRequest>
{
    public TaoNhatKyValidator()
    {
        RuleFor(x => x.BatchId).NotEmpty().WithMessage("BatchId không được để trống");
        RuleFor(x => x.HoatDong).NotEmpty().MaximumLength(200).WithMessage("Hoạt động không được để trống");
        RuleFor(x => x.NgayThucHien).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Ngày thực hiện không hợp lệ");
    }
}
