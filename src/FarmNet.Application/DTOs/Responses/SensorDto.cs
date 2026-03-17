namespace FarmNet.Application.DTOs.Responses;

public record SensorDto(Guid Id, string Ten, string? MoTa, bool HoatDong, DateTime NgayLapDat, Guid BatchId, string TenBatch)
{
    public SensorDto() : this(default, string.Empty, null, default, default, default, string.Empty) { }
}
