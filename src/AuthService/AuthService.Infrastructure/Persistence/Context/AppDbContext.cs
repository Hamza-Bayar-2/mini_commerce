using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserCredential> UserCredentials { get; set; }
    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_users");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ_users_email").IsUnique();
            entity.HasIndex(e => e.PhoneNumber, "UQ_users_phone_number").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("id");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2")
                .HasColumnName("created_at");

            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime2")
                .HasColumnName("deleted_at");

            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");

            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_user_credentials");

            entity.ToTable("user_credentials");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");

            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.User)
                .WithOne(p => p.UserCredential)
                .HasForeignKey<UserCredential>(d => d.UserId)
                .HasConstraintName("FK_user_credentials_users");
        });

        modelBuilder.Entity<UserRefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_user_refresh_tokens");

            entity.ToTable("user_refresh_tokens");

            entity.HasIndex(e => e.TokenHash, "UQ_user_refresh_tokens_token_hash").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("id");

            entity.Property(e => e.ClientIp)
                .HasMaxLength(45)
                .HasColumnName("client_ip");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2")
                .HasColumnName("created_at");

            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime2")
                .HasColumnName("expires_at");

            entity.Property(e => e.IsRevoked)
                .HasDefaultValue(false)
                .HasColumnName("is_revoked");

            entity.Property(e => e.LastUsedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2")
                .HasColumnName("last_used_at");

            entity.Property(e => e.ReplacedBy)
                .HasColumnName("replaced_by");

            entity.Property(e => e.TokenHash)
                .HasColumnName("token_hash");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id");

            entity.HasOne(d => d.ReplacedByNavigation)
                .WithMany(p => p.InverseReplacedByNavigation)
                .HasForeignKey(d => d.ReplacedBy)
                .OnDelete(DeleteBehavior.Restrict)  // SQL Server CASCADE döngüsel FK'da sorun çıkarır
                .HasConstraintName("FK_refresh_tokens_replaced_by");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserRefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_refresh_tokens_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}