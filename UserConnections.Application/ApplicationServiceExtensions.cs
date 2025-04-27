using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace UserConnections.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register all MediatR handlers from the Application assembly
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        return services;
    }
} 