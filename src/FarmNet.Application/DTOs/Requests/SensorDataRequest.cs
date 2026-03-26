namespace FarmNet.Application.DTOs.Requests;

public record SensorDataRequest(
    string DeviceId,
    string BatchId,
    DateTime ThoiGian,
    double Temperature,
    double Humidity,
    bool CoMua,
    double SoilMoisture,
    double Gas,
    bool PumpOn,
    byte[]? HinhAnh = null
);
