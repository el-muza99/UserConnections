using Microsoft.EntityFrameworkCore;
using UserConnections.Infrastructure.Entities;

namespace UserConnections.Infrastructure.Persistence;

public class UserConnectionDbContext : DbContext
{
    public DbSet<UserConnectionEntity> UserConnections { get; set; } = null!;
    public DbSet<ConnectionEventOutbox> ConnectionEvents { get; set; } = null!;

    public UserConnectionDbContext(DbContextOptions<UserConnectionDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConnectionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
} 