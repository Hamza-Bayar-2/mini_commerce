using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;

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
            new ProductStatus { Id = (short)ProductStatuses.ACTIVE, Name = ProductStatuses.ACTIVE.ToString(), Description = "Product is available for sale" },
            new ProductStatus { Id = (short)ProductStatuses.INACTIVE, Name = ProductStatuses.INACTIVE.ToString(), Description = "Product is not available" },
            new ProductStatus { Id = (short)ProductStatuses.DRAFT, Name = ProductStatuses.DRAFT.ToString(), Description = "Product is being edited" },
            new ProductStatus { Id = (short)ProductStatuses.BANNED, Name = ProductStatuses.BANNED.ToString(), Description = "Product is prohibited" },
            new ProductStatus { Id = (short)ProductStatuses.OUT_OF_STOCK, Name = ProductStatuses.OUT_OF_STOCK.ToString(), Description = "Product is currently out of stock" }
        );
    }
}
