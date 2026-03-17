namespace FarmNet.Application.DTOs.Requests;

public record SensorDataRequest(
    string BatchId,
    double Temperature,
    double Humidity,
    double SoilPH,
    double LightLevel,
    double SoilMoisture
);
