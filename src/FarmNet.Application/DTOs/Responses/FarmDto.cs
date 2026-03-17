namespace FarmNet.Application.DTOs.Responses;

public record FarmDto(Guid Id, string Ten, string DiaChi, string? MoTa, DateTime NgayTao, int SoLuongLo)
{
    public FarmDto() : this(default, string.Empty, string.Empty, null, default, default) { }
}
