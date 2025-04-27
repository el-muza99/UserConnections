using NSubstitute;
using UserConnections.Application.Handlers;
using UserConnections.Application.Repositories;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Tests.Application;

public class GetLastUserConnectionHandlerTests
{
    private readonly IUserConnectionRepository _mockRepository;
    private readonly GetLastUserConnectionHandler _handler;

    public GetLastUserConnectionHandlerTests()
    {
        _mockRepository = Substitute.For<IUserConnectionRepository>();
        _handler = new GetLastUserConnectionHandler(_mockRepository);
    }

    [Fact]
    public async Task Handle_WithExistingUser_ReturnsLastConnection()
    {
        // Arrange
        var userId = 123L;
        var query = new GetLastUserConnectionQuery(userId);
        
        var ipAddress = "192.168.1.1";
        var connectionTime = DateTime.UtcNow;
        var expectedConnection = UserConnectionInfo.Create(userId, ipAddress, connectionTime);
        
        _mockRepository.GetLastUserConnectionAsync(userId, default)
            .Returns(expectedConnection);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(expectedConnection, result);
        await _mockRepository.Received(1).GetLastUserConnectionAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistingUser_ReturnsNull()
    {
        // Arrange
        var userId = 456L;
        var query = new GetLastUserConnectionQuery(userId);
        
        _mockRepository.GetLastUserConnectionAsync(userId, default)
            .Returns((UserConnectionInfo?)null);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
        await _mockRepository.Received(1).GetLastUserConnectionAsync(userId, Arg.Any<CancellationToken>());
    }
} 