using Microsoft.AspNetCore.Http;

namespace FarmNet.Api.Models;

public class SensorDataFormRequest
{
    public string BatchId { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double SoilPH { get; set; }
    public double LightLevel { get; set; }
    public double SoilMoisture { get; set; }

    public IFormFile? Image { get; set; }
}
