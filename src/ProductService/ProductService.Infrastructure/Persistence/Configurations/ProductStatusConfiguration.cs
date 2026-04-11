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
    }
}
