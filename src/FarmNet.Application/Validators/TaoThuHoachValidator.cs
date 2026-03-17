using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class TaoThuHoachValidator : AbstractValidator<TaoThuHoachRequest>
{
    public TaoThuHoachValidator()
    {
        RuleFor(x => x.BatchId).NotEmpty().WithMessage("BatchId không được để trống");
        RuleFor(x => x.NgayThuHoach).LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Ngày thu hoạch không hợp lệ");
        RuleFor(x => x.TrongLuong).GreaterThan(0).WithMessage("Trọng lượng phải lớn hơn 0");
    }
}
