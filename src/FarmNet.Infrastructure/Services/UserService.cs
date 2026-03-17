using FluentResults;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.DTOs.Responses;
using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Services;

public class UserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task<IEnumerable<NguoiDungDto>> GetAllAsync()
    {
        var users = await userManager.Users.Include(u => u.Farm).ToListAsync();
        var result = new List<NguoiDungDto>();
        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            result.Add(new NguoiDungDto(u.Id, u.HoTen, u.Email!, roles.FirstOrDefault() ?? "", u.FarmId, u.Farm?.Ten));
        }
        return result;
    }

    public async Task<NguoiDungDto?> GetByIdAsync(string id)
    {
        var user = await userManager.Users.Include(u => u.Farm).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return null;
        var roles = await userManager.GetRolesAsync(user);
        return new NguoiDungDto(user.Id, user.HoTen, user.Email!, roles.FirstOrDefault() ?? "", user.FarmId, user.Farm?.Ten);
    }

    public async Task<Result<NguoiDungDto>> CreateAsync(TaoNguoiDungRequest request)
    {
        var user = new AppUser
        {
            HoTen = request.HoTen,
            Email = request.Email,
            UserName = request.Email,
            FarmId = request.FarmId
        };

        var createResult = await userManager.CreateAsync(user, request.MatKhau);
        if (!createResult.Succeeded)
            return Result.Fail(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, request.VaiTro);
        return Result.Ok(new NguoiDungDto(user.Id, user.HoTen, user.Email!, request.VaiTro, user.FarmId, null));
    }

    public async Task<Result<NguoiDungDto>> UpdateAsync(string id, CapNhatNguoiDungRequest request)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Result.Fail("Không tìm thấy người dùng");

        user.HoTen = request.HoTen;
        user.Email = request.Email;
        user.UserName = request.Email;
        user.FarmId = request.FarmId;

        if (!string.IsNullOrEmpty(request.MatKhauMoi))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var pwResult = await userManager.ResetPasswordAsync(user, token, request.MatKhauMoi);
            if (!pwResult.Succeeded)
                return Result.Fail(string.Join(", ", pwResult.Errors.Select(e => e.Description)));
        }

        await userManager.UpdateAsync(user);
        var roles = await userManager.GetRolesAsync(user);
        return Result.Ok(new NguoiDungDto(user.Id, user.HoTen, user.Email!, roles.FirstOrDefault() ?? "", user.FarmId, null));
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Result.Fail("Không tìm thấy người dùng");
        await userManager.DeleteAsync(user);
        return Result.Ok();
    }
}
