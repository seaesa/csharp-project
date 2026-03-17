using AutoMapper;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class TraceabilityService(
    IRepository<Batch> batchRepo,
    IRepository<SensorData> sensorDataRepo,
    IRepository<BlockchainRecord> blockchainRepo,
    IBlockchainService blockchain,
    IMapper mapper) : ITraceabilityService
{
    public async Task<TraceabilityDto?> GetTraceAsync(string batchMaLo)
    {
        var batch = await batchRepo.Query()
            .Include(b => b.Farm)
            .Include(b => b.NhatKy).ThenInclude(l => l.NguoiThucHien)
            .Include(b => b.ThuHoach)
            .Include(b => b.BlockchainRecords)
            .FirstOrDefaultAsync(b => b.MaLo == batchMaLo);

        if (batch == null) return null;

        var sensorData = await GetSensorDataAsync(batchMaLo);

        return new TraceabilityDto(
            batch.MaLo,
            batch.TenSanPham,
            batch.Farm.Ten,
            batch.Farm.DiaChi,
            LayTenTrangThai(batch.TrangThai),
            batch.NgayTao,
            mapper.Map<List<FarmingLogDto>>(batch.NhatKy.OrderBy(l => l.NgayThucHien).ToList()),
            batch.ThuHoach == null ? null : mapper.Map<HarvestDto>(batch.ThuHoach),
            mapper.Map<List<SensorDataDto>>(sensorData),
            mapper.Map<List<BlockchainRecordDto>>(batch.BlockchainRecords.OrderBy(r => r.ThoiGian).ToList())
        );
    }

    public async Task<bool> VerifyBatchAsync(Guid batchId)
    {
        var records = await blockchainRepo.Query()
            .Where(r => r.BatchId == batchId && r.TxHash != null)
            .ToListAsync();

        if (!records.Any()) return false;

        foreach (var record in records)
        {
            if (!await blockchain.VerifyHashAsync(record.TxHash!, record.DataHash))
                return false;
        }
        return true;
    }

    private async Task<IEnumerable<SensorData>> GetSensorDataAsync(string batchMaLo)
    {
        return await sensorDataRepo.Query()
            .Where(d => d.BatchId == batchMaLo)
            .OrderByDescending(d => d.ThoiGian)
            .Take(20)
            .ToListAsync();
    }

    private static string LayTenTrangThai(Domain.Enums.BatchStatus status) => status switch
    {
        Domain.Enums.BatchStatus.DangCanhTac => "Đang canh tác",
        Domain.Enums.BatchStatus.DaThuHoach => "Đã thu hoạch",
        Domain.Enums.BatchStatus.DaDongGoi => "Đã đóng gói",
        Domain.Enums.BatchStatus.DaBan => "Đã bán",
        _ => "Không xác định"
    };
}
