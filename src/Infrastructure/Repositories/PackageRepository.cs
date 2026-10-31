using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PackageRepository(AppDbContext db) : IPackageRepository
{
    public Task<Package?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Packages.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<List<Package>> GetAllAsync(CancellationToken ct = default)
        => (await db.Packages.AsNoTracking().ToListAsync(ct)).OrderBy(p => p.Name).ToList();

    public Task<bool> AnyContainingProductAsync(Guid productId, CancellationToken ct = default)
        => db.Packages.AnyAsync(p => p.Items.Any(i => i.ProductId == productId), ct);

    public async Task AddAsync(Package package, CancellationToken ct = default)
        => await db.Packages.AddAsync(package, ct);

    public void Remove(Package package) => db.Packages.Remove(package);
}
