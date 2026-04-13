using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LogService.Domain.Entities;

namespace LogService.Infrastructure.Persistence.Configurations;

public sealed class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK_logs");

        builder.ToTable("logs");

        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()")
            .HasColumnName("id");

        builder.Property(e => e.Message)
            .IsRequired()
            .HasColumnName("message");
            
        builder.Property(e => e.ServiceName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("service_name");

        builder.Property(e => e.CreatedAt) 
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnType("datetime2")
            .HasColumnName("created_at");
    }
}
