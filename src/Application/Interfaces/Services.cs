using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductDto>> SearchAsync(string? query, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderDto> ConfirmAsync(Guid id, CancellationToken ct = default);
    Task<OrderDto> CancelAsync(Guid id, CancellationToken ct = default);
}

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IPackageService
{
    Task<List<PackageDto>> GetAllAsync(CancellationToken ct = default);
    Task<PackageDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PackageDto> CreateAsync(CreatePackageRequest request, CancellationToken ct = default);
    Task<PackageDto> UpdateAsync(Guid id, UpdatePackageRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
