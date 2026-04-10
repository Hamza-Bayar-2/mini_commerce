using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasKey(e => e.Id).HasName("PK_users");

    builder.ToTable("users");

    builder.HasIndex(e => e.Email, "UQ_users_email").IsUnique();
    builder.HasIndex(e => e.PhoneNumber, "UQ_users_phone_number").IsUnique();

    builder.Property(e => e.Id)
        .HasDefaultValueSql("NEWID()")
        .HasColumnName("id");

    builder.Property(e => e.CreatedAt)
        .HasDefaultValueSql("GETUTCDATE()")
        .HasColumnType("datetime2")
        .HasColumnName("created_at");

    builder.Property(e => e.DeletedAt)
        .HasColumnType("datetime2")
        .HasColumnName("deleted_at");

    builder.Property(e => e.Email)
        .HasMaxLength(255)
        .HasColumnName("email");

    builder.Property(e => e.FirstName)
        .HasMaxLength(50)
        .HasColumnName("first_name");

    builder.Property(e => e.LastName)
        .HasMaxLength(50)
        .HasColumnName("last_name");

    builder.Property(e => e.PhoneNumber)
        .HasMaxLength(20)
        .HasColumnName("phone_number");

    builder.HasMany(d => d.Roles)
        .WithMany(p => p.Users)
        .UsingEntity<Dictionary<string, object>>(
            "user_roles",
            j => j.HasOne<Role>().WithMany().HasForeignKey("role_id").OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_user_roles_roles"),
            j => j.HasOne<User>().WithMany().HasForeignKey("user_id").OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_user_roles_users"),
            j =>
            {
              j.HasKey("user_id", "role_id").HasName("PK_user_roles");
              j.ToTable("user_roles");
            });
  }
}