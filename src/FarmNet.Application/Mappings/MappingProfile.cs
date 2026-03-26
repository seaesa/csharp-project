using AutoMapper;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Enums;

namespace FarmNet.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Farm, FarmDto>()
            .ForMember(d => d.SoLuongLo, o => o.MapFrom(s => s.Batches.Count));

        CreateMap<Batch, BatchDto>()
            .ForMember(d => d.TenFarm, o => o.MapFrom(s => s.Farm.Ten))
            .ForMember(d => d.TenTrangThai, o => o.MapFrom(s => LayTenTrangThai(s.TrangThai)));

        CreateMap<Sensor, SensorDto>()
            .ForMember(d => d.TenBatch, o => o.MapFrom(s => s.Batch.MaLo));

        CreateMap<SensorData, SensorDataDto>()
            .ForMember(d => d.NhietDo, o => o.MapFrom(s => s.NhietDo))
            .ForMember(d => d.DoAm, o => o.MapFrom(s => s.DoAm))
            .ForMember(d => d.DoAmDat, o => o.MapFrom(s => s.DoAmDat))
            .ForMember(d => d.CoMua, o => o.MapFrom(s => s.CoMua))
            .ForMember(d => d.KhiGas, o => o.MapFrom(s => s.KhiGas))
            .ForMember(d => d.BomBat, o => o.MapFrom(s => s.BomBat))
            .ForMember(d => d.HinhAnhBase64, o => o.MapFrom(s => s.HinhAnh != null ? Convert.ToBase64String(s.HinhAnh) : null));

        CreateMap<FarmingLog, FarmingLogDto>()
            .ForMember(d => d.TenNguoiThucHien, o => o.MapFrom(s => s.NguoiThucHien.HoTen));

        CreateMap<Harvest, HarvestDto>();

        CreateMap<BlockchainRecord, BlockchainRecordDto>()
            .ForMember(d => d.TenSuKien, o => o.MapFrom(s => LayTenSuKien(s.LoaiSuKien)));
    }

    private static string LayTenTrangThai(BatchStatus status) => status switch
    {
        BatchStatus.DangCanhTac => "Đang canh tác",
        BatchStatus.DaThuHoach => "Đã thu hoạch",
        BatchStatus.DaDongGoi => "Đã đóng gói",
        BatchStatus.DaBan => "Đã bán",
        _ => "Không xác định"
    };

    private static string LayTenSuKien(BlockchainEventType type) => type switch
    {
        BlockchainEventType.TaoLo => "Tạo lô sản phẩm",
        BlockchainEventType.NhatKyCanhTac => "Nhật ký canh tác",
        BlockchainEventType.TongHopCamBien => "Tổng hợp cảm biến",
        BlockchainEventType.ThuHoach => "Thu hoạch",
        BlockchainEventType.TongHopHangNgay => "Tổng hợp hàng ngày",
        _ => "Không xác định"
    };
}
