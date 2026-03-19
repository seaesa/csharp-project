using FarmNet.Domain.Enums;

namespace FarmNet.Application.DTOs.Responses;

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

public record BlockchainVerifyItemDto(
    Guid RecordId,
    BlockchainEventType LoaiSuKien,
    string TenSuKien,
    string StoredHash,
    string ComputedHash,
    bool IsMatch,
    bool DaXacNhan,
    string? TxHash,
    VerifyStatus TrangThaiXacThuc,
    DateTime ThoiGian
);

public record BlockchainVerifyResultDto(
    bool ToanVen,
    int TongSoBanGhi,
    int SoBanGhiHopLe,
    int SoBanGhiBiThayDoi,
    int SoBanGhiChuaXacNhan,
    List<BlockchainVerifyItemDto> ChiTiet
);
