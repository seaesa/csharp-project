namespace FarmNet.Application.DTOs.Responses;

public enum VerifyStatus { HopLe, ChuaXacNhan, BiThayDoi, BlockchainKhongKhop, KhongTruyCapBlockchain }

public record DailyVerifyItemDto(
    DateOnly Ngay,
    string StoredHash,
    string ComputedHash,
    bool IsMatch,
    int SoNhatKy,
    int SoCamBien,
    string? TxHash,
    VerifyStatus TrangThai
);

public record BlockchainVerifyResultDto(
    bool ToanVen,
    int TongSoNgay,
    int SoNgayKiemTra,
    int SoNgayHopLe,
    int SoNgayBiThayDoi,
    VerifyStatus TaoLoStatus,
    VerifyStatus? ThuHoachStatus,
    List<DailyVerifyItemDto> ChiTietNgay
);

public record DailyHashDto(
    Guid Id,
    DateOnly Ngay,
    string DataHash,
    string? TxHash,
    bool DaXacNhan,
    int SoNhatKy,
    int SoCamBien,
    DateTime NgayTao
);
