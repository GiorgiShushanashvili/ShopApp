using Domain.Exceptions;

namespace Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public static Product Create(string name, decimal price, int stock)
    {
        var product = new Product { Id = Guid.NewGuid() };
        product.Update(name, price, stock);
        return product;
    }

    public void Update(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Product name is required.");
        if (name.Trim().Length > 200) throw new DomainException("Product name cannot exceed 200 characters.");
        if (price <= 0) throw new DomainException("Price must be greater than zero.");
        if (stock < 0) throw new DomainException("Stock cannot be negative.");

        Name = name.Trim();
        Price = price;
        Stock = stock;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (Stock < quantity) throw new DomainException($"Insufficient stock for '{Name}'.");
        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        Stock += quantity;
    }
}
