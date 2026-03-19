using FarmNet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        string[] roles = ["Admin", "FarmOwner", "Worker"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (!await userManager.Users.AnyAsync())
        {
            var admin = new AppUser
            {
                HoTen = "Quản trị viên",
                Email = "admin@gmail.com",
                UserName = "admin@gmail.com"
            };
            await userManager.CreateAsync(admin, "admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
