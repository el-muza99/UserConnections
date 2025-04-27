using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserConnections.Domain.Aggregates;
using UserConnections.Domain.ValueObjects;
using UserConnections.Infrastructure.Persistence;
using UserConnections.Infrastructure.Repositories;

namespace UserConnections.Tests.Infrastructure;

public class UserConnectionRepositoryTests
{
    [Fact]
    public async Task UpsertAsync_ValidUserConnection_AddsToContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UserConnectionDbContext>()
            .UseInMemoryDatabase(databaseName: $"UserConnectionDb_{Guid.NewGuid()}")
            .Options;

        using var context = new UserConnectionDbContext(options);
        var repository = new UserConnectionRepository(context);
        
        var userId = 123L;
        var ipAddress = IpAddress.Create("192.168.1.1");
        var connectionTime = DateTime.UtcNow;
        var userConnection = UserConnection.Create(userId, ipAddress, connectionTime);
        
        // Act
        await repository.UpsertAsync(new[] { userConnection });
        
        // Assert
        var savedConnection = await context.UserConnections.FirstOrDefaultAsync();
        Assert.NotNull(savedConnection);
        Assert.Equal(userId, savedConnection.UserId);
        Assert.Equal(ipAddress.Value, savedConnection.IpAddress);
    }
} 