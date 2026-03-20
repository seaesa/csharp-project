using FarmNet.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FarmNet.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorData> SensorData => Set<SensorData>();
    public DbSet<FarmingLog> FarmingLogs => Set<FarmingLog>();
    public DbSet<Harvest> Harvests => Set<Harvest>();
    public DbSet<BlockchainRecord> BlockchainRecords => Set<BlockchainRecord>();
    public DbSet<DailyHash> DailyHashes => Set<DailyHash>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Farm>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Ten).IsRequired().HasMaxLength(200);
            e.Property(x => x.DiaChi).IsRequired().HasMaxLength(500);
        });

        builder.Entity<Batch>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.MaLo).IsUnique();
            e.Property(x => x.MaLo).IsRequired().HasMaxLength(50);
            e.Property(x => x.TenSanPham).IsRequired().HasMaxLength(200);
            e.HasOne(x => x.Farm).WithMany(x => x.Batches).HasForeignKey(x => x.FarmId);
        });

        builder.Entity<Sensor>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Batch).WithMany(x => x.Sensors).HasForeignKey(x => x.BatchId);
        });

        builder.Entity<SensorData>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Sensor).WithMany(x => x.DuLieu).HasForeignKey(x => x.SensorId).IsRequired(false);
        });

        builder.Entity<FarmingLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Batch).WithMany(x => x.NhatKy).HasForeignKey(x => x.BatchId);
            e.HasOne(x => x.NguoiThucHien).WithMany().HasForeignKey(x => x.NguoiThucHienId);
        });

        builder.Entity<Harvest>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Batch).WithOne(x => x.ThuHoach).HasForeignKey<Harvest>(x => x.BatchId);
        });

        builder.Entity<BlockchainRecord>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Batch).WithMany(x => x.BlockchainRecords).HasForeignKey(x => x.BatchId);
        });

        builder.Entity<DailyHash>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.BatchId, x.Ngay }).IsUnique();
            e.HasOne(x => x.Batch).WithMany(x => x.DailyHashes).HasForeignKey(x => x.BatchId);
        });

        builder.Entity<AppUser>(e =>
        {
            e.HasOne(x => x.Farm).WithMany(x => x.NguoiDung).HasForeignKey(x => x.FarmId).IsRequired(false);
        });
    }
}
