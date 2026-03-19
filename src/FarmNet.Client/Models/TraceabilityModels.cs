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
    ChuaXacNhanBlockchain = 1,
    DbBiThayDoi = 2,
    BlockchainKhongKhop = 3,
    KhongTimThayDuLieu = 4,
    KhongTruyCapBlockchain = 5,
    KhongHoTroLoaiSuKien = 6,
}

public class BlockchainVerifyItemDto
{
    public Guid RecordId { get; set; }
    public int LoaiSuKien { get; set; }
    public string TenSuKien { get; set; } = string.Empty;
    public string StoredHash { get; set; } = string.Empty;
    public string ComputedHash { get; set; } = string.Empty;
    public bool IsMatch { get; set; }
    public bool DaXacNhan { get; set; }
    public string? TxHash { get; set; }
    public VerifyStatus TrangThaiXacThuc { get; set; }
    public DateTime ThoiGian { get; set; }
}

public class BlockchainVerifyResultDto
{
    public bool ToanVen { get; set; }
    public int TongSoBanGhi { get; set; }
    public int SoBanGhiHopLe { get; set; }
    public int SoBanGhiBiThayDoi { get; set; }
    public int SoBanGhiChuaXacNhan { get; set; }
    public List<BlockchainVerifyItemDto> ChiTiet { get; set; } = [];
}
