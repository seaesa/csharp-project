using FarmNet.Application.Services;
using FarmNet.Domain.Entities;
using FarmNet.Domain.Interfaces;
using FarmNet.Infrastructure.Data;
using FarmNet.Infrastructure.Repositories;
using FarmNet.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FarmNet.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddIdentity<AppUser, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = true;
            opt.Password.RequiredLength = 6;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IBlockchainService, BlockchainService>();
        services.AddScoped<IDailyHashService, DailyHashService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFarmService, FarmService>();
        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<ISensorService, SensorService>();
        services.AddScoped<IFarmingLogService, FarmingLogService>();
        services.AddScoped<IHarvestService, HarvestService>();
        services.AddScoped<ITraceabilityService, TraceabilityService>();

        return services;
    }
}
