using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<Category>> GetAllAsync(CancellationToken ct = default)
        => (await db.Categories.AsNoTracking().ToListAsync(ct)).OrderBy(c => c.Name).ToList();

    public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var normalized = name.Trim().ToLower();
        return db.Categories.AnyAsync(c => c.Name.ToLower() == normalized && c.Id != excludeId, ct);
    }

    public async Task AddAsync(Category category, CancellationToken ct = default)
        => await db.Categories.AddAsync(category, ct);

    public void Remove(Category category) => db.Categories.Remove(category);
}
