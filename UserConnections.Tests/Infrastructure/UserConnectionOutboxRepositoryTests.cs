using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserConnections.Domain.Events;
using UserConnections.Domain.ValueObjects;
using UserConnections.Infrastructure.Persistence;
using UserConnections.Infrastructure.Repositories;
using UserConnections.Infrastructure.Entities;

namespace UserConnections.Tests.Infrastructure;

public class UserConnectionOutboxRepositoryTests
{
    [Fact]
    public async Task SaveAsync_ValidConnectionEvent_AddsToContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UserConnectionDbContext>()
            .UseInMemoryDatabase(databaseName: $"OutboxDb_{Guid.NewGuid()}")
            .Options;

        using var context = new UserConnectionDbContext(options);
        var repository = new UserConnectionOutboxRepository(context);
        
        var userId = 123L;
        var ipAddress = IpAddress.Create("192.168.1.1");
        var connectionTime = DateTime.UtcNow;
        var connectionEvent = new ConnectionEvent(userId, ipAddress, connectionTime);
        
        // Act
        await repository.SaveAsync(connectionEvent, CancellationToken.None);
        
        // Assert
        var savedEvent = await context.ConnectionEvents.FirstOrDefaultAsync();
        Assert.NotNull(savedEvent);
        Assert.Equal(userId, savedEvent.UserId);
        Assert.Equal(ipAddress.Value, savedEvent.IpAddress);
        Assert.Equal(connectionTime, savedEvent.ConnectionTimeUtc);
    }
} 