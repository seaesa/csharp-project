namespace FarmNet.Application.DTOs.Responses;

public record SensorDataDto(
    Guid Id,
    string BatchId,
    double NhietDo,
    double DoAm,
    double DoPH,
    double AnhSang,
    double DoAmDat,
    DateTime ThoiGian
)
{
    public SensorDataDto() : this(default, string.Empty, default, default, default, default, default, default) { }
}
