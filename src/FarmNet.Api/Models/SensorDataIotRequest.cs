using System.Text.Json.Serialization;

namespace FarmNet.Api.Models;

public class SensorDataIotRequest
{
    [JsonPropertyName("device")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("batchID")]
    public string BatchId { get; set; } = string.Empty;

    // Format from ESP32 RTC: "yyyy-MM-dd HH:mm:ss" (UTC)
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("hum")]
    public double Hum { get; set; }

    // IoT spec: rain = 1 => không mưa, rain = 0 => mưa
    [JsonPropertyName("rain")]
    public int Rain { get; set; }

    // IoT spec: analog soil moisture (ADC raw)
    [JsonPropertyName("water")]
    public double Water { get; set; }

    // IoT spec: MQ7 gas (analog raw)
    [JsonPropertyName("gas")]
    public double Gas { get; set; }

    // IoT spec: pump = 0/1
    [JsonPropertyName("pump")]
    public int Pump { get; set; }
}

