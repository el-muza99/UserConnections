using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserConnections.Application.Repositories;
using UserConnections.Infrastructure.Persistence;
using UserConnections.Infrastructure.Repositories;
using UserConnections.Infrastructure.Services;
using UserConnections.Infrastructure.Settings;

namespace UserConnections.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register DbContext using connection string from API layer's configuration
        services.AddDbContext<UserConnectionDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("UserConnectionsDb"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());
        });
        
        services.Configure<ConnectionProcessorSettings>(
            configuration.GetSection(ConnectionProcessorSettings.SectionName));
        
        services.AddHostedService<ConnectionEventProcessor>();

        services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();
        services.AddScoped<IUserConnectionOutboxRepository, UserConnectionOutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}

