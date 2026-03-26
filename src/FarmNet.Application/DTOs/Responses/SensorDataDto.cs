namespace FarmNet.Application.DTOs.Responses;

public record SensorDataDto(
    Guid Id,
    string BatchId,
    double NhietDo,
    double DoAm,
    double DoAmDat,
    bool CoMua,
    double KhiGas,
    bool BomBat,
    DateTime ThoiGian,
    string? HinhAnhBase64
)
{
    public SensorDataDto() : this(default, string.Empty, default, default, default, false, default, false, default, null) { }
}
