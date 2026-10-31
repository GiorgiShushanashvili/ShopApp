using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;

namespace Application.Services;

public class CategoryService(ICategoryRepository categories, IUnitOfWork uow) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        => (await categories.GetAllAsync(ct)).Select(c => c.ToDto()).ToList();

    public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await Find(id, ct)).ToDto();

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest r, CancellationToken ct = default)
    {
        var category = Category.Create(r.Name, r.Description);
        await EnsureUniqueName(category.Name, null, ct);
        await categories.AddAsync(category, ct);
        await uow.SaveChangesAsync(ct);
        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest r, CancellationToken ct = default)
    {
        var category = await Find(id, ct);
        category.Update(r.Name, r.Description);
        await EnsureUniqueName(category.Name, id, ct);
        await uow.SaveChangesAsync(ct);
        return category.ToDto();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await Find(id, ct);
        categories.Remove(category);
        await uow.SaveChangesAsync(ct);
    }

    private async Task EnsureUniqueName(string name, Guid? excludeId, CancellationToken ct)
    {
        if (await categories.NameExistsAsync(name, excludeId, ct))
            throw new DomainException($"A category named '{name}' already exists.");
    }

    private async Task<Category> Find(Guid id, CancellationToken ct)
        => await categories.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Category), id);
}
