using Domain.Entities;
using Domain.Enums;
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

    public Task<bool> AnyPendingContainingProductAsync(Guid productId, CancellationToken ct = default)
        => db.Orders.AnyAsync(o => o.Status == OrderStatus.Pending && o.Items.Any(i => i.ProductId == productId), ct);

    public async Task AddAsync(Order order, CancellationToken ct = default)
        => await db.Orders.AddAsync(order, ct);
}
