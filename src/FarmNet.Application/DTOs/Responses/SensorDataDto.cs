namespace FarmNet.Application.DTOs.Responses;

public record SensorDataDto(
    Guid Id,
    string BatchId,
    double NhietDo,
    double DoAm,
    double DoPH,
    double AnhSang,
    double DoAmDat,
    DateTime ThoiGian,
    string? HinhAnhBase64
)
{
    public SensorDataDto() : this(default, string.Empty, default, default, default, default, default, default, null) { }
}
