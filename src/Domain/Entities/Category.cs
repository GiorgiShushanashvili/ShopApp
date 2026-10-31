using Domain.Exceptions;

namespace Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private Category() { }

    public static Category Create(string name, string? description)
    {
        var category = new Category { Id = Guid.NewGuid() };
        category.Update(name, description);
        return category;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Category name is required.");
        if (name.Trim().Length > 100) throw new DomainException("Category name cannot exceed 100 characters.");
        if (description?.Trim().Length > 500) throw new DomainException("Category description cannot exceed 500 characters.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
