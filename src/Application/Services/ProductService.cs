using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public class ProductService(IProductRepository products, IPackageRepository packages, IOrderRepository orders, IUnitOfWork uow) : IProductService
{
    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
        => (await products.GetAllAsync(ct)).Select(p => p.ToDto()).ToList();

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await Find(id, ct)).ToDto();

    public async Task<List<ProductDto>> SearchAsync(string? query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new DomainException("Search query 'q' is required.");

        var term = query.Trim();
        return (await products.GetAllAsync(ct))
            .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.ToDto())
            .ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest r, CancellationToken ct = default)
    {
        var product = Product.Create(r.Name, r.Price, r.Stock);
        await products.AddAsync(product, ct);
        await uow.SaveChangesAsync(ct);
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest r, CancellationToken ct = default)
    {
        var product = await Find(id, ct);
        product.Update(r.Name, r.Price, r.Stock);
        await uow.SaveChangesAsync(ct);
        return product.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await Find(id, ct);
        if (await packages.AnyContainingProductAsync(id, ct))
            throw new DomainException($"Product '{product.Name}' is part of a package and cannot be deleted.");
        if (await orders.AnyPendingContainingProductAsync(id, ct))
            throw new DomainException($"Product '{product.Name}' is in a pending order and cannot be deleted.");
        products.Remove(product);
        await uow.SaveChangesAsync(ct);
    }

    private async Task<Product> Find(Guid id, CancellationToken ct)
        => await products.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Product), id);
}
