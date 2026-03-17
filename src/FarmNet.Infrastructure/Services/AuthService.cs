using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FarmNet.Infrastructure.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtService jwtService) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail("Email hoặc mật khẩu không đúng");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.MatKhau, false);
        if (!result.Succeeded)
            return Result.Fail("Email hoặc mật khẩu không đúng");

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtService.GenerateToken(user, roles);
        var hetHan = DateTime.UtcNow.AddHours(24);

        return Result.Ok(new AuthResponse(token, user.Email!, user.HoTen, roles.FirstOrDefault() ?? "", hetHan));
    }
}
