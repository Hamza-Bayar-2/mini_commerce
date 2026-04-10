using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_user_refresh_tokens");

        builder.ToTable("user_refresh_tokens");

        builder.HasIndex(e => e.TokenHash, "UQ_user_refresh_tokens_token_hash").IsUnique();

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .HasColumnName("id");

        builder.Property(e => e.ClientIp)
            .HasMaxLength(45)
            .HasColumnName("client_ip");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnType("datetime2")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnType("datetime2")
            .HasColumnName("expires_at");

        builder.Property(e => e.IsRevoked)
            .HasDefaultValue(false)
            .HasColumnName("is_revoked");

        builder.Property(e => e.LastUsedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnType("datetime2")
            .HasColumnName("last_used_at");

        builder.Property(e => e.ReplacedBy)
            .HasColumnName("replaced_by");

        builder.Property(e => e.TokenHash)
            .HasColumnName("token_hash");

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.HasOne(d => d.ReplacedByNavigation)
            .WithMany(p => p.InverseReplacedByNavigation)
            .HasForeignKey(d => d.ReplacedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_refresh_tokens_replaced_by");

        builder.HasOne(d => d.User)
            .WithMany(p => p.UserRefreshTokens)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("FK_refresh_tokens_users");
    }
}
