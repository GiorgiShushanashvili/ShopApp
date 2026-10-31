using Application.DTOs;
using Domain.Entities;

namespace Application.Services;

internal static class Mapping
{
    public static ProductDto ToDto(this Product p) => new(p.Id, p.Name, p.Price, p.Stock);

    public static CategoryDto ToDto(this Category c) => new(c.Id, c.Name, c.Description);

    public static OrderDto ToDto(this Order o) => new(
        o.Id, o.CreatedAt, o.Status, o.Total,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList());

    public static PackageDto ToDto(this Package p, IReadOnlyDictionary<Guid, Product> products)
    {
        var items = p.Items.Select(i =>
        {
            var product = products[i.ProductId];
            return new PackageItemDto(i.ProductId, product.Name, product.Price, i.Quantity, product.Price * i.Quantity);
        }).ToList();

        var itemsTotal = items.Sum(i => i.LineTotal);
        var available = p.Items.Count == 0 ? 0 : p.Items.Min(i => products[i.ProductId].Stock / i.Quantity);
        return new PackageDto(p.Id, p.Name, p.Price, p.CreatedAt, itemsTotal, itemsTotal - p.Price, available, items);
    }
}
