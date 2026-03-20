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

public enum VerifyStatus
{
    HopLe = 0,
    ChuaXacNhan = 1,
    BiThayDoi = 2,
    BlockchainKhongKhop = 3,
    KhongTruyCapBlockchain = 4,
}

public class DailyVerifyItemDto
{
    public string Ngay { get; set; } = string.Empty;
    public string StoredHash { get; set; } = string.Empty;
    public string ComputedHash { get; set; } = string.Empty;
    public bool IsMatch { get; set; }
    public int SoNhatKy { get; set; }
    public int SoCamBien { get; set; }
    public string? TxHash { get; set; }
    public VerifyStatus TrangThai { get; set; }
}

public class BlockchainVerifyResultDto
{
    public bool ToanVen { get; set; }
    public int TongSoNgay { get; set; }
    public int SoNgayKiemTra { get; set; }
    public int SoNgayHopLe { get; set; }
    public int SoNgayBiThayDoi { get; set; }
    public VerifyStatus TaoLoStatus { get; set; }
    public VerifyStatus? ThuHoachStatus { get; set; }
    public List<DailyVerifyItemDto> ChiTietNgay { get; set; } = [];
}
