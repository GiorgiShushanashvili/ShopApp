using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.Property(p => p.Price).HasPrecision(18, 2);
        // Optimistic concurrency: prevents lost updates (overselling) when concurrent orders decrement stock.
        b.Property(p => p.Stock).IsConcurrencyToken();
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.HasKey(o => o.Id);
        // Optimistic concurrency: prevents two concurrent cancels from both restocking.
        b.Property(o => o.Status).HasConversion<string>().HasMaxLength(20).IsConcurrencyToken();
        b.Ignore(o => o.Total);

        b.OwnsMany(o => o.Items, ib =>
        {
            ib.ToTable("OrderItems");
            ib.WithOwner().HasForeignKey("OrderId");
            ib.HasKey(i => i.Id);
            ib.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
            ib.Property(i => i.UnitPrice).HasPrecision(18, 2);
            ib.Ignore(i => i.LineTotal);
        });

        b.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).IsRequired().HasMaxLength(200);
        b.Property(p => p.Price).HasPrecision(18, 2);

        b.OwnsMany(p => p.Items, ib =>
        {
            ib.ToTable("PackageItems");
            ib.WithOwner().HasForeignKey("PackageId");
            ib.HasKey(i => i.Id);
            ib.Property(i => i.Id).ValueGeneratedNever(); // items are replaced on update -> must be INSERTed
            ib.HasOne<Product>().WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Navigation(p => p.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Name).IsRequired().HasMaxLength(100).UseCollation("NOCASE");
        b.HasIndex(c => c.Name).IsUnique();
        b.Property(c => c.Description).HasMaxLength(500);
    }
}
