using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// A composite package: a bundle of existing products (each with a quantity) sold at its own price.
/// </summary>
public class Package
{
    private readonly List<PackageItem> _items = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<PackageItem> Items => _items.AsReadOnly();

    private Package() { }

    public static Package Create(string name, decimal price, IEnumerable<(Guid ProductId, int Quantity)> items)
    {
        var package = new Package { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        package.Update(name, price, items);
        return package;
    }

    public void Update(string name, decimal price, IEnumerable<(Guid ProductId, int Quantity)> items)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Package name is required.");
        if (price <= 0) throw new DomainException("Price must be greater than zero.");

        var list = items?.ToList() ?? new();
        if (list.Count == 0) throw new DomainException("A package must contain at least one product.");
        if (list.Any(i => i.Quantity <= 0)) throw new DomainException("Quantity must be greater than zero.");
        if (list.Select(i => i.ProductId).Distinct().Count() != list.Count)
            throw new DomainException("A product can appear only once in a package.");

        Name = name.Trim();
        Price = price;
        _items.Clear();
        _items.AddRange(list.Select(i => new PackageItem(i.ProductId, i.Quantity)));
    }
}

public class PackageItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    private PackageItem() { }

    internal PackageItem(Guid productId, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
    }
}
