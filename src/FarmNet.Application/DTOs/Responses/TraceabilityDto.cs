namespace FarmNet.Application.DTOs.Responses;

public record TraceabilityDto(
    string MaLo,
    string TenSanPham,
    string TenFarm,
    string DiaChiFarm,
    string TrangThai,
    DateTime NgayTao,
    List<FarmingLogDto> NhatKyCanhTac,
    HarvestDto? ThuHoach,
    List<SensorDataDto> DuLieuCamBien,
    List<BlockchainRecordDto> BlockchainRecords
);
