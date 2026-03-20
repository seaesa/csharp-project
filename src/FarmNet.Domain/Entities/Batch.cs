using FarmNet.Domain.Enums;

namespace FarmNet.Domain.Entities;

public class Batch
{
    public Guid Id { get; set; }
    public string MaLo { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public BatchStatus TrangThai { get; set; } = BatchStatus.DangCanhTac;
    public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    public DateTime? NgayCapNhat { get; set; }

    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = null!;

    public ICollection<Sensor> Sensors { get; set; } = [];
    public ICollection<FarmingLog> NhatKy { get; set; } = [];
    public Harvest? ThuHoach { get; set; }
    public ICollection<BlockchainRecord> BlockchainRecords { get; set; } = [];
    public ICollection<DailyHash> DailyHashes { get; set; } = [];
}
