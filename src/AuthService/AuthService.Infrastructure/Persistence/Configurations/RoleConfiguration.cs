using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_roles");

        builder.ToTable("roles");

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.HasData(
            new Role { Id = (short)Roles.CUSTOMER, Name = Roles.CUSTOMER.ToString() },
            new Role { Id = (short)Roles.SELLER, Name = Roles.SELLER.ToString() },
            new Role { Id = (short)Roles.ADMIN, Name = Roles.ADMIN.ToString() },
            new Role { Id = (short)Roles.EDITOR, Name = Roles.EDITOR.ToString() }
        );
    }
}
