using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(i => i.LineTotal);

    private Order() { }

    public static Order Create() => new()
    {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Status = OrderStatus.Pending
    };

    public void AddItem(Product product, int quantity)
    {
        EnsurePending();

        if (_items.Any(i => i.ProductId == product.Id))
            throw new DomainException($"Product '{product.Name}' is already in the order.");

        product.DecreaseStock(quantity);
        _items.Add(new OrderItem(product, quantity));
    }

    public void Confirm()
    {
        EnsurePending();
        if (_items.Count == 0) throw new DomainException("Cannot confirm an empty order.");
        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled) throw new DomainException("Order is already cancelled.");
        if (Status == OrderStatus.Confirmed) throw new DomainException("Confirmed orders cannot be cancelled.");
        Status = OrderStatus.Cancelled;
    }

    private void EnsurePending()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Only pending orders can be modified.");
    }
}
