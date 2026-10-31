using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
        => (await db.Products.AsNoTracking().ToListAsync(ct)).OrderBy(p => p.Name).ToList();

    public Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var set = ids.Distinct().ToList();
        return db.Products.AsNoTracking().Where(p => set.Contains(p.Id)).ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
        => await db.Products.AddAsync(product, ct);

    public void Remove(Product product) => db.Products.Remove(product);
}
