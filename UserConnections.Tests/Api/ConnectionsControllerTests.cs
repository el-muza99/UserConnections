using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserConnections.Api.Controllers;
using UserConnections.Api.Dtos;
using UserConnections.Api.Models;
using UserConnections.Application.Handlers;
using UserConnections.Domain.UserConnectionInfo;
using Xunit;

namespace UserConnections.Tests.Api;

public class ConnectionsControllerTests
{
    private readonly ConnectionsController _controller;
    private readonly IMediator _mediator;

    public ConnectionsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new ConnectionsController(_mediator);
    }
    

    [Fact]
    public async Task CreateConnection_WithEmptyIpAddress_ReturnsBadRequest()
    {
        // Arrange
        var userId = 123L;
        var request = new CreateUserConnectionRequest
        {
            UserId = userId,
            IpAddress = ""
        };

        // Act
        var result = await _controller.CreateConnection(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorArray = Assert.IsType<object[]>(badRequestResult.Value);
        Assert.Equal("IP address is required", errorArray[0]);
    }



    [Fact]
    public async Task GetUserIps_WithValidUserId_ReturnsOk()
    {
        // Arrange
        var userId = 123L;
        var connections = new List<UserConnectionInfo>
        {
            UserConnectionInfo.Create(userId, "192.168.1.1", DateTime.UtcNow.AddDays(-1)),
            UserConnectionInfo.Create(userId, "192.168.1.2", DateTime.UtcNow)
        };
        
        var result = new GetUserIpsResult(userId, connections);
        _mediator.Send(Arg.Any<GetUserIpsQuery>(), Arg.Any<CancellationToken>())
            .Returns(result);

        // Act
        var response = await _controller.GetUserIps(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        var userIpsResponse = Assert.IsType<UserIpsResponse>(okResult.Value);
        Assert.Equal(userId, userIpsResponse.UserId);
        Assert.Equal(connections.Count, userIpsResponse.Connections.Count);
    }

    [Fact]
    public async Task GetUserIps_WithEmptyResults_ReturnsNotFound()
    {
        // Arrange
        var userId = 456L;
        _mediator.Send(Arg.Any<GetUserIpsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetUserIpsResult(userId, new List<UserConnectionInfo>()));

        // Act
        var result = await _controller.GetUserIps(userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorArray = Assert.IsType<object[]>(notFoundResult.Value);
        Assert.Contains($"No connections found for user {userId}", errorArray[0].ToString());
    }

    [Fact]
    public async Task GetLastConnection_WithValidUserId_ReturnsOk()
    {
        // Arrange
        var userId = 123L;
        var ipAddress = "192.168.1.1";
        var lastConnectionTime = DateTime.UtcNow;
        var connection = UserConnectionInfo.Create(userId, ipAddress, lastConnectionTime);
        
        _mediator.Send(Arg.Any<GetLastUserConnectionQuery>(), Arg.Any<CancellationToken>())
            .Returns(connection);

        // Act
        var result = await _controller.GetLastConnection(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LastConnectionResponse>(okResult.Value);
        Assert.Equal(userId, response.UserId);
        Assert.Equal(ipAddress, response.IpAddress);
        Assert.Equal(lastConnectionTime, response.LastConnectionUtc);
    }

    [Fact]
    public async Task GetLastConnection_WithNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = 456L;
        UserConnectionInfo? nullConnection = null;
        _mediator.Send(Arg.Any<GetLastUserConnectionQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(nullConnection));

        // Act
        var result = await _controller.GetLastConnection(userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorArray = Assert.IsType<object[]>(notFoundResult.Value);
        Assert.Contains($"No connections found for user {userId}", errorArray[0].ToString());
    }
} 