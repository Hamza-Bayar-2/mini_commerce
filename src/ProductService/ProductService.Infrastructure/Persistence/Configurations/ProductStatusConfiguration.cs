using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence.Configurations;

public class ProductStatusConfiguration : IEntityTypeConfiguration<ProductStatus>
{
    public void Configure(EntityTypeBuilder<ProductStatus> builder)
    {
        builder.ToTable("product_statuses");

        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Id)
            .HasColumnName("id");

        builder.Property(ps => ps.Name)
            .HasColumnName("name")
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(ps => ps.Name)
            .IsUnique();

        builder.Property(ps => ps.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.HasData(
            new ProductStatus { Id = 1, Name = "ACTIVE", Description = "Product is available for sale" },
            new ProductStatus { Id = 2, Name = "INACTIVE", Description = "Product is not available" },
            new ProductStatus { Id = 3, Name = "DRAFT", Description = "Product is being edited" },
            new ProductStatus { Id = 4, Name = "BANNED", Description = "Product is prohibited" },
            new ProductStatus { Id = 5, Name = "OUT_OF_STOCK", Description = "Product is currently out of stock" }
        );
    }
}
