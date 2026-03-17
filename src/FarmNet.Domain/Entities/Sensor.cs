namespace FarmNet.Domain.Entities;

public class Sensor
{
    public Guid Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool HoatDong { get; set; } = true;
    public DateTime NgayLapDat { get; set; } = DateTime.UtcNow;

    public Guid BatchId { get; set; }
    public Batch Batch { get; set; } = null!;

    public ICollection<SensorData> DuLieu { get; set; } = [];
}
