using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("NEWID()");

        builder.Property(p => p.StatusId)
            .HasColumnName("status_id")
            .IsRequired();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(p => p.Stock)
            .HasColumnName("stock")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("datetime2");

        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("datetime2");

        builder.HasOne(p => p.Status)
            .WithMany(ps => ps.Products)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
