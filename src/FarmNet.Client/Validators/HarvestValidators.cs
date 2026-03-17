using FluentValidation;
using FarmNet.Client.Models;

namespace FarmNet.Client.Validators;

public class TaoThuHoachRequestValidator : AbstractValidator<TaoThuHoachRequest>
{
    public TaoThuHoachRequestValidator()
    {
        RuleFor(x => x.NgayThuHoach)
            .NotEmpty().WithMessage("Vui lòng chọn ngày thu hoạch");

        RuleFor(x => x.TrongLuong)
            .GreaterThan(0).WithMessage("Trọng lượng phải lớn hơn 0");

        RuleFor(x => x.GhiChuChatLuong)
            .MaximumLength(500).WithMessage("Ghi chú tối đa 500 ký tự");
    }
}
