using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Aggregates;
using UserConnections.Domain.ValueObjects;
using UserConnections.Infrastructure.Persistence;

namespace UserConnections.Infrastructure.Services;

public class ConnectionEventProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ConnectionEventProcessor> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);

    public ConnectionEventProcessor(
        IServiceProvider serviceProvider,
        ILogger<ConnectionEventProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Connection event processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEventsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing connection events");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }
    }

    private async Task ProcessPendingEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserConnectionDbContext>();
        var connectionRepository = scope.ServiceProvider.GetRequiredService<IUserConnectionRepository>();
        
        const int batchSize = 100;
        
        var pendingEvents = await dbContext.ConnectionEvents
            .Where(e => e.ProcessedAtUtc == null)
            .OrderBy(e => e.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
            
        if (!pendingEvents.Any())
            return;
            
        _logger.LogInformation("Processing {Count} connection events", pendingEvents.Count);
        
        foreach (var eventItem in pendingEvents)
        {
            try
            {
                var ipAddress = IpAddress.Create(eventItem.IpAddress);
                var userConnection = UserConnection.Create(
                    eventItem.UserId,
                    ipAddress,
                    eventItem.ConnectionTimeUtc);
                    
                await connectionRepository.UpsertAsync(userConnection, cancellationToken);
                
                eventItem.ProcessedAtUtc = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process connection event {EventId}", eventItem.Id);
            }
        }
    }
} 