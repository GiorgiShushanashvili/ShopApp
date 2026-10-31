using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public class OrderService(IOrderRepository orders, IProductRepository products, IUnitOfWork uow) : IOrderService
{
    public async Task<List<OrderDto>> GetAllAsync(CancellationToken ct = default)
        => (await orders.GetAllAsync(ct)).Select(o => o.ToDto()).ToList();

    public async Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await Find(id, ct)).ToDto();

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new DomainException("An order must contain at least one item.");

        var order = Order.Create();

        foreach (var line in request.Items)
        {
            var product = await products.GetByIdAsync(line.ProductId, ct)
                          ?? throw new NotFoundException(nameof(Product), line.ProductId);
            order.AddItem(product, line.Quantity);
        }

        await orders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);
        return order.ToDto();
    }

    public async Task<OrderDto> ConfirmAsync(Guid id, CancellationToken ct = default)
    {
        var order = await Find(id, ct);
        order.Confirm();
        await uow.SaveChangesAsync(ct);
        return order.ToDto();
    }

    public async Task<OrderDto> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var order = await Find(id, ct);
        order.Cancel();

        // give the stock back
        foreach (var item in order.Items)
        {
            var product = await products.GetByIdAsync(item.ProductId, ct);
            product?.IncreaseStock(item.Quantity);
        }

        await uow.SaveChangesAsync(ct);
        return order.ToDto();
    }

    private async Task<Order> Find(Guid id, CancellationToken ct)
        => await orders.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Order), id);
}
