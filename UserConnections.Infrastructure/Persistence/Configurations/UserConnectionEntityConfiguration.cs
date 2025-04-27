using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;
using UserConnections.Infrastructure.Entities;

namespace UserConnections.Infrastructure.Persistence.Configurations;

public class UserConnectionEntityConfiguration : IEntityTypeConfiguration<UserConnectionEntity>
{
    public void Configure(EntityTypeBuilder<UserConnectionEntity> builder)
    {
        builder.ToTable("UserConnections");
        
        builder.HasKey(x => new { x.UserId, x.IpAddress });
        
        builder.Property(x => x.UserId)
            .HasColumnName("UserId");
        
        builder.Property(x => x.IpAddress)
            .HasColumnName("IpAddress")
            .HasColumnType("varchar")
            .HasMaxLength(45);
        
        builder.Property(x => x.LastConnectionUtc)
            .HasColumnName("LastConnectionUtc");
            
        builder.HasIndex(x => x.IpAddress);
    }
} 