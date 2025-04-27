using NSubstitute;
using UserConnections.Application.Handlers;
using UserConnections.Application.Repositories;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Tests.Application;

public class GetLastConnectionByIpHandlerTests
{
    private readonly IUserConnectionRepository _mockRepository;
    private readonly GetLastConnectionByIpHandler _handler;

    public GetLastConnectionByIpHandlerTests()
    {
        _mockRepository = Substitute.For<IUserConnectionRepository>();
        _handler = new GetLastConnectionByIpHandler(_mockRepository);
    }

    [Fact]
    public async Task Handle_WithValidIp_ReturnsLastConnection()
    {
        // Arrange
        var ipString = "192.168.1.1";
        var query = new GetLastConnectionByIpQuery(ipString);
        
        var userId = 123L;
        var connectionTime = DateTime.UtcNow;
        var expectedConnection = UserConnectionInfo.Create(userId, ipString, connectionTime);
        
        _mockRepository.GetLastConnectionByIpAsync(ipString, default)
            .Returns(expectedConnection);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(expectedConnection, result);
    }

    [Fact]
    public async Task Handle_WithNonExistingIp_ReturnsNull()
    {
        // Arrange
        var ipString = "192.168.1.100";
        var query = new GetLastConnectionByIpQuery(ipString);
        
        _mockRepository.GetLastConnectionByIpAsync(ipString, default)
            .Returns((UserConnectionInfo?)null);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Null(result);
    }
} 