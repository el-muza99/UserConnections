using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;
using UserConnections.Infrastructure.Entities;

namespace UserConnections.Infrastructure.Persistence.Configurations;

public class ConnectionEventOutboxConfiguration : IEntityTypeConfiguration<ConnectionEventOutbox>
{
    public void Configure(EntityTypeBuilder<ConnectionEventOutbox> builder)
    {
        builder.ToTable("ConnectionEventsOutbox");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.UserId)
            .HasColumnName("UserId");
        
        builder.Property(x => x.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("varchar")
            .HasMaxLength(45);
        
        builder.Property(x => x.ConnectionTimeUtc)
            .HasColumnName("ConnectionTimeUtc");
        
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc");
        
        builder.Property(x => x.ProcessedAtUtc)
            .HasColumnName("ProcessedAtUtc")
            .IsRequired(false);
            
        builder.HasIndex(x => x.ProcessedAtUtc);
    }
} 