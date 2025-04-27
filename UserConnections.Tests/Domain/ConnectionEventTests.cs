using UserConnections.Domain.Events;
using UserConnections.Domain.ValueObjects;

namespace UserConnections.Tests.Domain;

public class ConnectionEventTests
{
    [Fact]
    public void Create_WithValidInputs_CreatesConnectionEvent()
    {
        // Arrange
        var userId = 123L;
        var ipAddress = IpAddress.Create("192.168.1.1");
        var connectedAtUtc = DateTime.UtcNow;
        
        // Act
        var connectionEvent = ConnectionEvent.Create(userId, ipAddress, connectedAtUtc);
        
        // Assert
        Assert.NotNull(connectionEvent);
        Assert.Equal(userId, connectionEvent.UserId);
        Assert.Equal(ipAddress, connectionEvent.IpAddress);
        Assert.Equal(connectedAtUtc, connectionEvent.ConnectedAtUtc);
    }
    
    [Fact]
    public void Create_WithInvalidUserId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var invalidUserId = 0L;
        var ipAddress = IpAddress.Create("192.168.1.1");
        var connectedAtUtc = DateTime.UtcNow;
        
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => 
            ConnectionEvent.Create(invalidUserId, ipAddress, connectedAtUtc));
        
        Assert.Equal("userId", exception.ParamName);
        Assert.Contains("Value must be greater than or equal to 1", exception.Message);
    }
} 