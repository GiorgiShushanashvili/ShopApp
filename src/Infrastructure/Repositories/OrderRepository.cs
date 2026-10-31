using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<List<Order>> GetAllAsync(CancellationToken ct = default)
        => (await db.Orders.AsNoTracking().ToListAsync(ct)).OrderByDescending(o => o.CreatedAt).ToList();

    public async Task AddAsync(Order order, CancellationToken ct = default)
        => await db.Orders.AddAsync(order, ct);
}
