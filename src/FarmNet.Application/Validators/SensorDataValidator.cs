using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class SensorDataValidator : AbstractValidator<SensorDataRequest>
{
    public SensorDataValidator()
    {
        RuleFor(x => x.BatchId).NotEmpty().WithMessage("BatchId không được để trống");
        RuleFor(x => x.ThoiGian).NotEqual(default(DateTime)).WithMessage("time không hợp lệ");
        RuleFor(x => x.Temperature).InclusiveBetween(-50, 100).WithMessage("Nhiệt độ phải từ -50°C đến 100°C");
        RuleFor(x => x.Humidity).InclusiveBetween(0, 100).WithMessage("Độ ẩm phải từ 0% đến 100%");
        // water: analog (ADC). Chưa có spec tuyệt đối nên chỉ validate dải hợp lý.
        RuleFor(x => x.SoilMoisture).GreaterThanOrEqualTo(0).WithMessage("water phải >= 0");
        RuleFor(x => x.SoilMoisture).LessThanOrEqualTo(5000).WithMessage("water phải <= 5000");
        // gas: MQ7 analog
        RuleFor(x => x.Gas).GreaterThanOrEqualTo(0).WithMessage("gas phải >= 0");
        RuleFor(x => x.Gas).LessThanOrEqualTo(5000).WithMessage("gas phải <= 5000");
    }
}
