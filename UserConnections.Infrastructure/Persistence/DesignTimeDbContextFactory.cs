using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserConnections.Infrastructure.Persistence;

/// <summary>
/// Factory to create DbContext instances during design-time operations like migrations.
/// This is not used at runtime, only for EF Core tooling.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserConnectionDbContext>
{
    public UserConnectionDbContext CreateDbContext(string[] args)
    {
        // Hardcoded connection string for migrations only
        // The real connection string should be in the API layer's appsettings.json
        const string connectionString = "Host=localhost;Database=user_connections;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<UserConnectionDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new UserConnectionDbContext(optionsBuilder.Options);
    }
} 