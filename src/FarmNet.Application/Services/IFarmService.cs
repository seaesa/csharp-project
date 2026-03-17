using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface IFarmService
{
    Task<IEnumerable<FarmDto>> GetAllAsync();
    Task<FarmDto?> GetByIdAsync(Guid id);
    Task<Result<FarmDto>> CreateAsync(TaoFarmRequest request);
    Task<Result<FarmDto>> UpdateAsync(Guid id, TaoFarmRequest request);
    Task<Result> DeleteAsync(Guid id);
}
