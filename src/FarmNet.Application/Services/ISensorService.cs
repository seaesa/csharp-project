using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface ISensorService
{
    Task<IEnumerable<SensorDto>> GetAllAsync(Guid? batchId = null);
    Task<SensorDto?> GetByIdAsync(Guid id);
    Task<Result<SensorDto>> CreateAsync(TaoSensorRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> RecordDataAsync(SensorDataRequest request);
    Task<IEnumerable<SensorDataDto>> GetDataByBatchAsync(string batchMaLo);
    Task<IEnumerable<SensorDataDto>> GetRecentDataAsync(int count = 50);
}
