namespace Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { }

    internal OrderItem(Product product, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = product.Id;
        ProductName = product.Name;
        UnitPrice = product.Price;   // price snapshot at order time
        Quantity = quantity;
    }
}
