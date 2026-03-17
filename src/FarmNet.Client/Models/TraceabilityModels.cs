namespace FarmNet.Client.Models;

public class TraceabilityDto
{
    public string MaLo { get; set; } = string.Empty;
    public string TenSanPham { get; set; } = string.Empty;
    public string TenFarm { get; set; } = string.Empty;
    public string DiaChiFarm { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }
    public List<FarmingLogDto> NhatKyCanhTac { get; set; } = [];
    public HarvestDto? ThuHoach { get; set; }
    public List<SensorDataDto> DuLieuCamBien { get; set; } = [];
    public List<BlockchainRecordDto> BlockchainRecords { get; set; } = [];
}

public class BlockchainRecordDto
{
    public Guid Id { get; set; }
    public int LoaiSuKien { get; set; }
    public string TenSuKien { get; set; } = string.Empty;
    public string DataHash { get; set; } = string.Empty;
    public string? TxHash { get; set; }
    public bool DaXacNhan { get; set; }
    public DateTime ThoiGian { get; set; }
}
