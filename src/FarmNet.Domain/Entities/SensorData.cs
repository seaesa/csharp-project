namespace FarmNet.Domain.Entities;

public class SensorData
{
    public Guid Id { get; set; }
    public string BatchId { get; set; } = string.Empty;
    public double NhietDo { get; set; }
    public double DoAm { get; set; }
    public double DoPH { get; set; }
    public double AnhSang { get; set; }
    public double DoAmDat { get; set; }
    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;

    public Guid? SensorId { get; set; }
    public Sensor? Sensor { get; set; }
}
