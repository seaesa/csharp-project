using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface IUserService
{
    Task<IEnumerable<NguoiDungDto>> GetAllAsync();
    Task<NguoiDungDto?> GetByIdAsync(string id);
    Task<Result<NguoiDungDto>> CreateAsync(TaoNguoiDungRequest request);
    Task<Result<NguoiDungDto>> UpdateAsync(string id, CapNhatNguoiDungRequest request);
    Task<Result> DeleteAsync(string id);
}
