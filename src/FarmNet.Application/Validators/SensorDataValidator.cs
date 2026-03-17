using FluentValidation;
using FarmNet.Application.DTOs.Requests;

namespace FarmNet.Application.Validators;

public class SensorDataValidator : AbstractValidator<SensorDataRequest>
{
    public SensorDataValidator()
    {
        RuleFor(x => x.BatchId).NotEmpty().WithMessage("BatchId không được để trống");
        RuleFor(x => x.Temperature).InclusiveBetween(-50, 100).WithMessage("Nhiệt độ phải từ -50°C đến 100°C");
        RuleFor(x => x.Humidity).InclusiveBetween(0, 100).WithMessage("Độ ẩm phải từ 0% đến 100%");
        RuleFor(x => x.SoilPH).InclusiveBetween(0, 14).WithMessage("Độ pH phải từ 0 đến 14");
        RuleFor(x => x.LightLevel).GreaterThanOrEqualTo(0).WithMessage("Cường độ ánh sáng không hợp lệ");
        RuleFor(x => x.SoilMoisture).InclusiveBetween(0, 100).WithMessage("Độ ẩm đất phải từ 0% đến 100%");
    }
}
