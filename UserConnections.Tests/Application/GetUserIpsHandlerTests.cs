using NSubstitute;
using UserConnections.Application.Handlers;
using UserConnections.Application.Repositories;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Tests.Application;

public class GetUserIpsHandlerTests
{
    private readonly IUserConnectionRepository _mockRepository;
    private readonly GetUserIpsHandler _handler;

    public GetUserIpsHandlerTests()
    {
        _mockRepository = Substitute.For<IUserConnectionRepository>();
        _handler = new GetUserIpsHandler(_mockRepository);
    }

    [Fact]
    public async Task Handle_WithExistingUser_ReturnsUserIps()
    {
        // Arrange
        var userId = 123L;
        var query = new GetUserIpsQuery(userId);
        
        var connections = new List<UserConnectionInfo> 
        { 
            UserConnectionInfo.Create(userId, "192.168.1.1", DateTime.UtcNow.AddDays(-1)),
            UserConnectionInfo.Create(userId, "192.168.1.2", DateTime.UtcNow) 
        };
        
        _mockRepository.GetUserIpsAsync(userId, default).Returns(connections);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(userId, result.UserId);
        Assert.Equal(connections.Count, result.Connections.Count());
        Assert.Contains(connections[0], result.Connections);
        Assert.Contains(connections[1], result.Connections);
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ReturnsEmptyList()
    {
        // Arrange
        var userId = 456L;
        var query = new GetUserIpsQuery(userId);
        
        _mockRepository.GetUserIpsAsync(userId, default).Returns(new List<UserConnectionInfo>());
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(userId, result.UserId);
        Assert.Empty(result.Connections);
    }
} 