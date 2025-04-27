using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Aggregates;
using UserConnections.Domain.ValueObjects;
using UserConnections.Infrastructure.Persistence;
using UserConnections.Infrastructure.Settings;
using System.Collections.Generic;

namespace UserConnections.Infrastructure.Services;

public class ConnectionEventProcessor(
    IServiceProvider serviceProvider,
    IOptions<ConnectionProcessorSettings> settings,
    ILogger<ConnectionEventProcessor> logger)
    : BackgroundService
{
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);
    private readonly ConnectionProcessorSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Connection event processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEventsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing connection events");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }
    }

    private async Task ProcessPendingEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserConnectionDbContext>();
        var connectionRepository = scope.ServiceProvider.GetRequiredService<IUserConnectionRepository>();
        
        var pendingEvents = await dbContext.ConnectionEvents
            .Where(e => e.ProcessedAtUtc == null)
            .OrderBy(e => e.CreatedAtUtc)
            .Take(_settings.BatchSize)
            .ToListAsync(cancellationToken);
            
        if (!pendingEvents.Any())
            return;
            
        logger.LogInformation("Processing {Count} connection events", pendingEvents.Count);
        
        var userConnections = new List<UserConnection>();
        var processedEventIds = new List<Guid>();
        
        foreach (var eventItem in pendingEvents)
        {
            try
            {
                var ipAddress = IpAddress.Create(eventItem.IpAddress);
                var userConnection = UserConnection.Create(
                    eventItem.UserId,
                    ipAddress,
                    eventItem.ConnectionTimeUtc);
                    
                userConnections.Add(userConnection);
                processedEventIds.Add(eventItem.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process connection event {EventId}", eventItem.Id);
            }
        }
        
        if (userConnections.Any())
        {
            try
            {
                await connectionRepository.UpsertAsync(userConnections, cancellationToken);
                
                // Mark events as processed
                foreach (var eventId in processedEventIds)
                {
                    var eventItem = pendingEvents.First(e => e.Id == eventId);
                    eventItem.ProcessedAtUtc = DateTime.UtcNow;
                }
                
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save batch of {Count} connection events", userConnections.Count);
            }
        }
    }
} 