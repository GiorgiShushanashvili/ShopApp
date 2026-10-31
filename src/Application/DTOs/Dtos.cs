using Domain.Enums;

namespace Application.DTOs;

public record ProductDto(Guid Id, string Name, decimal Price, int Stock);
public record CreateProductRequest(string Name, decimal Price, int Stock);
public record UpdateProductRequest(string Name, decimal Price, int Stock);

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
public record OrderDto(Guid Id, DateTime CreatedAt, OrderStatus Status, decimal Total, List<OrderItemDto> Items);

public record OrderLineRequest(Guid ProductId, int Quantity);
public record CreateOrderRequest(List<OrderLineRequest> Items);

public record CategoryDto(Guid Id, string Name, string? Description);
public record CreateCategoryRequest(string Name, string? Description);
public record UpdateCategoryRequest(string Name, string? Description);

public record PackageItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
public record PackageDto(
    Guid Id, string Name, decimal Price,
    DateTime CreatedAt,
    decimal ItemsTotal,      // sum of the component products' current prices
    decimal Savings,         // ItemsTotal - Price
    int AvailableStock,      // how many packages can be assembled from current product stock
    List<PackageItemDto> Items);

public record PackageItemRequest(Guid ProductId, int Quantity);
public record CreatePackageRequest(string Name, decimal Price, List<PackageItemRequest> Items);
public record UpdatePackageRequest(string Name, decimal Price, List<PackageItemRequest> Items);
