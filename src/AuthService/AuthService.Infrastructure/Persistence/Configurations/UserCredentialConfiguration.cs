using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.HasKey(e => e.UserId).HasName("PK_user_credentials");

        builder.ToTable("user_credentials");

        builder.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");

        builder.Property(e => e.PasswordHash)
            .HasColumnName("password_hash");

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnType("datetime2")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.User)
            .WithOne(p => p.UserCredential)
            .HasForeignKey<UserCredential>(d => d.UserId)
            .HasConstraintName("FK_user_credentials_users");
    }
}
