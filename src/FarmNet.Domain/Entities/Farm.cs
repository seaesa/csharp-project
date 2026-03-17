namespace FarmNet.Domain.Entities;

public class Farm
{
    public Guid Id { get; set; }
    public string Ten { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;

    public ICollection<Batch> Batches { get; set; } = [];
    public ICollection<AppUser> NguoiDung { get; set; } = [];
}
