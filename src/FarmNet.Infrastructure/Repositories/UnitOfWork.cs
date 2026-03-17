using FarmNet.Domain.Interfaces;
using FarmNet.Infrastructure.Data;

namespace FarmNet.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        context.SaveChangesAsync(ct);

    public void Dispose() => context.Dispose();
}
