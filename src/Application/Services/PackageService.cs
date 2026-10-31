using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public class PackageService(IPackageRepository packages, IProductRepository products, IUnitOfWork uow) : IPackageService
{
    public async Task<List<PackageDto>> GetAllAsync(CancellationToken ct = default)
    {
        var all = await packages.GetAllAsync(ct);
        var lookup = await LoadProducts(all.SelectMany(p => p.Items.Select(i => i.ProductId)), ct);
        return all.Select(p => p.ToDto(lookup)).ToList();
    }

    public async Task<PackageDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await ToDto(await Find(id, ct), ct);

    public async Task<PackageDto> CreateAsync(CreatePackageRequest r, CancellationToken ct = default)
    {
        var items = await ValidateItems(r.Items, ct);
        var package = Package.Create(r.Name, r.Price, items);
        await packages.AddAsync(package, ct);
        await uow.SaveChangesAsync(ct);
        return await ToDto(package, ct);
    }

    public async Task<PackageDto> UpdateAsync(Guid id, UpdatePackageRequest r, CancellationToken ct = default)
    {
        var package = await Find(id, ct);
        var items = await ValidateItems(r.Items, ct);
        package.Update(r.Name, r.Price, items);
        await uow.SaveChangesAsync(ct);
        return await ToDto(package, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var package = await Find(id, ct);
        packages.Remove(package);
        await uow.SaveChangesAsync(ct);
    }

    // Ensures every referenced product exists; returns the lines in domain form.
    private async Task<List<(Guid ProductId, int Quantity)>> ValidateItems(List<PackageItemRequest>? lines, CancellationToken ct)
    {
        if (lines is null || lines.Count == 0)
            throw new DomainException("A package must contain at least one product.");

        var found = await LoadProducts(lines.Select(l => l.ProductId), ct);
        var missing = lines.FirstOrDefault(l => !found.ContainsKey(l.ProductId));
        if (missing is not null) throw new NotFoundException(nameof(Product), missing.ProductId);

        return lines.Select(l => (l.ProductId, l.Quantity)).ToList();
    }

    private async Task<PackageDto> ToDto(Package package, CancellationToken ct)
        => package.ToDto(await LoadProducts(package.Items.Select(i => i.ProductId), ct));

    private async Task<Dictionary<Guid, Product>> LoadProducts(IEnumerable<Guid> ids, CancellationToken ct)
        => (await products.GetByIdsAsync(ids, ct)).ToDictionary(p => p.Id);

    private async Task<Package> Find(Guid id, CancellationToken ct)
        => await packages.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Package), id);
}
