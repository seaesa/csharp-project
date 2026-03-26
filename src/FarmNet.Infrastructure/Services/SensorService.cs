using AutoMapper;
using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class SensorService(
    IRepository<Sensor> repo,
    IRepository<SensorData> dataRepo,
    IRepository<Batch> batchRepo,
    IUnitOfWork uow,
    IMapper mapper,
    IDailyHashService dailyHashService) : ISensorService
{
    public async Task<IEnumerable<SensorDto>> GetAllAsync(Guid? batchId = null)
    {
        var query = repo.Query().Include(s => s.Batch).AsQueryable();
        if (batchId.HasValue) query = query.Where(s => s.BatchId == batchId.Value);
        return mapper.Map<IEnumerable<SensorDto>>(await query.OrderByDescending(s => s.NgayLapDat).ToListAsync());
    }

    public async Task<SensorDto?> GetByIdAsync(Guid id)
    {
        var sensor = await repo.Query().Include(s => s.Batch).FirstOrDefaultAsync(s => s.Id == id);
        return sensor == null ? null : mapper.Map<SensorDto>(sensor);
    }

    public async Task<Result<SensorDto>> CreateAsync(TaoSensorRequest request)
    {
        var batch = await batchRepo.GetByIdAsync(request.BatchId);
        if (batch == null) return Result.Fail("Không tìm thấy lô sản phẩm");

        var sensor = new Sensor
        {
            Id = Guid.NewGuid(),
            Ten = request.Ten,
            MoTa = request.MoTa,
            BatchId = request.BatchId
        };
        await repo.AddAsync(sensor);
        await uow.SaveChangesAsync();

        var result = await repo.Query().Include(s => s.Batch).FirstOrDefaultAsync(s => s.Id == sensor.Id);
        return Result.Ok(mapper.Map<SensorDto>(result!));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var sensor = await repo.GetByIdAsync(id);
        if (sensor == null) return Result.Fail("Không tìm thấy cảm biến");
        repo.Remove(sensor);
        await uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> RecordDataAsync(SensorDataRequest request)
    {
        var batch = await batchRepo.Query().FirstOrDefaultAsync(b => b.MaLo == request.BatchId);
        if (batch == null) return Result.Fail($"Không tìm thấy lô sản phẩm '{request.BatchId}'");

        Sensor? sensor = null;
        if (!string.IsNullOrWhiteSpace(request.DeviceId))
        {
            sensor = await repo.Query()
                .FirstOrDefaultAsync(s =>
                    s.Ten == request.DeviceId &&
                    s.BatchId == batch.Id &&
                    s.HoatDong);
        }

        var data = new SensorData
        {
            Id = Guid.NewGuid(),
            BatchId = request.BatchId,
            NhietDo = request.Temperature,
            DoAm = request.Humidity,
            // Các cột pH/ánh sáng không nằm trong IoT spec hiện tại.
            DoPH = 0,
            AnhSang = 0,
            DoAmDat = request.SoilMoisture,
            CoMua = request.CoMua,
            KhiGas = request.Gas,
            BomBat = request.PumpOn,
            ThoiGian = request.ThoiGian,
            HinhAnh = request.HinhAnh,
            SensorId = sensor?.Id
        };

        await dataRepo.AddAsync(data);
        await uow.SaveChangesAsync();

        // Đồng bộ daily hash với dữ liệu cảm biến mới.
        // Nếu day đã từng commit blockchain trước đó, TxHash sẽ được giữ lại để verify phát hiện sai lệch.
        var ngayUtc = DateTime.SpecifyKind(data.ThoiGian, DateTimeKind.Utc);
        await dailyHashService.RecalcAsync(batch.Id, DateOnly.FromDateTime(ngayUtc));

        return Result.Ok();
    }

    public async Task<IEnumerable<SensorDataDto>> GetDataByBatchAsync(string batchMaLo)
    {
        var data = await dataRepo.Query()
            .Where(d => d.BatchId == batchMaLo)
            .OrderByDescending(d => d.ThoiGian)
            .ToListAsync();
        return mapper.Map<IEnumerable<SensorDataDto>>(data);
    }

    public async Task<IEnumerable<SensorDataDto>> GetRecentDataAsync(int count = 50)
    {
        var data = await dataRepo.Query()
            .OrderByDescending(d => d.ThoiGian)
            .Take(count)
            .ToListAsync();
        return mapper.Map<IEnumerable<SensorDataDto>>(data);
    }
}
