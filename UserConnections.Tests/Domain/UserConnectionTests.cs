using UserConnections.Domain.Aggregates;
using UserConnections.Domain.ValueObjects;

namespace UserConnections.Tests.Domain;

public class UserConnectionTests
{
    [Fact]
    public void Create_WithValidInputs_CreatesUserConnection()
    {
        // Arrange
        var userId = 123L;
        var ipAddress = IpAddress.Create("192.168.1.1");
        var connectionTime = DateTime.UtcNow;
        
        // Act
        var userConnection = UserConnection.Create(userId, ipAddress, connectionTime);
        
        // Assert
        Assert.NotNull(userConnection);
        Assert.Equal(userId, userConnection.UserId);
        Assert.Equal(ipAddress, userConnection.IpAddress);
        Assert.Equal(connectionTime, userConnection.LastConnectionUtc);
    }
    
} 