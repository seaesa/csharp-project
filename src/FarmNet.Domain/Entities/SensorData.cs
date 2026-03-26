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
    // IoT spec: rain = 1 => không mưa, rain = 0 => mưa
    public bool CoMua { get; set; }
    // IoT spec: gas (MQ7) - giá trị analog
    public double KhiGas { get; set; }
    // IoT spec: pump = 0/1
    public bool BomBat { get; set; }
    public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
    public byte[]? HinhAnh { get; set; }

    public Guid? SensorId { get; set; }
    public Sensor? Sensor { get; set; }
}
