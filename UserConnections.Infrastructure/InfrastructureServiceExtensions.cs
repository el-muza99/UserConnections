using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserConnections.Application.Repositories;
using UserConnections.Infrastructure.Persistence;
using UserConnections.Infrastructure.Repositories;
using UserConnections.Infrastructure.Services;

namespace UserConnections.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<UserConnectionDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("UserConnectionsDb"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());
        });
        
        services.AddHostedService<PostgresExtensionInitializer>();
        services.AddHostedService<ConnectionEventProcessor>();

        services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();
        services.AddScoped<IUserConnectionOutboxRepository, UserConnectionOutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}

public class PostgresExtensionInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public PostgresExtensionInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserConnectionDbContext>();
        
        await dbContext.Database.MigrateAsync(cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pg_trgm;", cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
} 