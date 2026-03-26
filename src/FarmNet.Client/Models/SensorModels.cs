namespace FarmNet.Client.Models;

public class SensorDto
{
    public Guid Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool HoatDong { get; set; }
    public DateTime NgayLapDat { get; set; }
    public Guid BatchId { get; set; }
    public string TenBatch { get; set; } = string.Empty;
}

public class SensorDataDto
{
    public Guid Id { get; set; }
    public string BatchId { get; set; } = string.Empty;
    public double NhietDo { get; set; }
    public double DoAm { get; set; }
    public double DoAmDat { get; set; }
    public bool CoMua { get; set; }
    public double KhiGas { get; set; }
    public bool BomBat { get; set; }
    public DateTime ThoiGian { get; set; }
}

public class TaoSensorRequest
{
    public string Ten { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public Guid BatchId { get; set; }
}
