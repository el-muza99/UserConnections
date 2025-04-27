using System.Net.Sockets;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using UserConnections.Application.Handlers;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Events;
using UserConnections.Domain.ValueObjects;
using Xunit;
using UserConnections.Domain.Aggregates;

namespace UserConnections.Tests.Application;

public class CreateUserConnectionHandlerTests
{
    private readonly IUserConnectionOutboxRepository _mockRepository;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly CreateUserConnectionHandler _handler;

    public CreateUserConnectionHandlerTests()
    {
        _mockRepository = Substitute.For<IUserConnectionOutboxRepository>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateUserConnectionHandler(_mockRepository, _mockUnitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidInput_CreatesUserConnection()
    {
        // Arrange
        var userId = 123L;
        var ipString = "192.168.1.1";
        var command = new CreateUserConnection(userId, ipString);
        
        // Act
        await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        await _mockRepository.Received(1).SaveAsync(
            Arg.Is<ConnectionEvent>(e => 
                e.UserId == userId && 
                e.IpAddress.Value == ipString),
            Arg.Any<CancellationToken>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_WithInvalidIpAddress_ThrowsArgumentException()
    {
        // Arrange
        var userId = 123L;
        var invalidIp = "invalid-ip";
        var command = new CreateUserConnection(userId, invalidIp);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        await _mockRepository.DidNotReceive().SaveAsync(
            Arg.Any<ConnectionEvent>(), 
            Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldSaveEventAndCommitTransaction()
    {
        // Arrange
        var userId = 123L;
        var ipString = "192.168.1.1";
        var command = new CreateUserConnection(userId, ipString);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _mockRepository.Received(1).SaveAsync(
            Arg.Is<ConnectionEvent>(e => 
                e.UserId == userId && 
                e.IpAddress.Value == ipString && 
                e.IpAddress.AddressFamily == AddressFamily.InterNetwork),
            Arg.Any<CancellationToken>());
        
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidIpv6Request_ShouldSaveEventWithCorrectAddressFamily()
    {
        // Arrange
        var userId = 123L;
        var ipString = "2001:db8::1";
        var command = new CreateUserConnection(userId, ipString);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _mockRepository.Received(1).SaveAsync(
            Arg.Is<ConnectionEvent>(e => 
                e.UserId == userId && 
                e.IpAddress.Value == ipString && 
                e.IpAddress.AddressFamily == AddressFamily.InterNetworkV6),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-ip")]
    [InlineData("http://example.com")]
    [InlineData("192.168.1.a")]
    public async Task Handle_WithInvalidIpAddress_ShouldThrowArgumentException(string invalidIp)
    {
        // Arrange
        var userId = 123L;
        var command = new CreateUserConnection(userId, invalidIp);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        // Verify no repository interactions occurred
        await _mockRepository.DidNotReceive().SaveAsync(
            Arg.Any<ConnectionEvent>(), 
            Arg.Any<CancellationToken>());
        
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_WithNullIpAddress_ShouldThrowArgumentException()
    {
        // Arrange
        string? nullIp = null;
        var userId = 123L;
        var command = new CreateUserConnection(userId, nullIp!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        // Verify no repository interactions occurred
        await _mockRepository.DidNotReceive().SaveAsync(
            Arg.Any<ConnectionEvent>(), 
            Arg.Any<CancellationToken>());
        
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
} 