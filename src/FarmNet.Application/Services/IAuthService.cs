using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;

namespace FarmNet.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}
