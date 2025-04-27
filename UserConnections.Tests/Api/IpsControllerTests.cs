using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UserConnections.Api.Controllers;
using UserConnections.Api.Dtos;
using UserConnections.Api.Models;
using UserConnections.Application.Handlers;
using UserConnections.Domain.UserConnectionInfo;
using Xunit;

namespace UserConnections.Tests.Api;

public class IpsControllerTests
{
    private readonly IpsController _controller;
    private readonly IMediator _mediator;
    private readonly ILogger<IpsController> _logger;

    public IpsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<IpsController>>();
        _controller = new IpsController(_mediator, _logger);
    }
    [Fact]
    public async Task GetLastConnectionByIp_WithValidIp_ReturnsOk()
    {
        // Arrange
        var ip = "192.168.1.1";
        var userId = 123L;
        var connectedAt = DateTime.UtcNow;
        var connection = UserConnectionInfo.Create(userId, ip, connectedAt);
        
        _mediator.Send(Arg.Any<GetLastConnectionByIpQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(connection));

        // Act
        var result = await _controller.GetLastConnectionByIp(ip);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserConnectionResponse>(okResult.Value);
        Assert.Equal(userId, response.UserId);
        Assert.Equal(connectedAt, response.LastConnectionUtc);
    }

    [Fact]
    public async Task GetLastConnectionByIp_WithNonExistingIp_ReturnsNotFound()
    {
        // Arrange
        var ip = "192.168.1.100";
        UserConnectionInfo? nullConnection = null;
        _mediator.Send(Arg.Any<GetLastConnectionByIpQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<UserConnectionInfo?>(nullConnection));

        // Act
        var result = await _controller.GetLastConnectionByIp(ip);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorArray = Assert.IsType<object[]>(notFoundResult.Value);
        Assert.Contains($"No connections found for IP {ip}", errorArray[0].ToString());
    }

    [Fact]
    public async Task FindUsersByIp_WithValidIp_ReturnsOk()
    {
        // Arrange
        var ip = "192.168.1";
        var page = 1;
        var pageSize = 10;
        var userIds = new List<long> { 1, 2, 3 };
        
        var response = new FindUsersByIpResult(userIds, userIds.Count, page, pageSize);
        _mediator.Send(Arg.Any<FindUsersByIpQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        // Act
        var result = await _controller.FindUsersByIp(ip, page, pageSize);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var usersResponse = Assert.IsType<UsersSearchResponse>(okResult.Value);
        Assert.Equal(userIds, usersResponse.UserIds);
        Assert.Equal(userIds.Count, usersResponse.TotalCount);
        Assert.Equal(page, usersResponse.Page);
        Assert.Equal(pageSize, usersResponse.PageSize);
    }

    [Fact]
    public async Task FindUsersByIp_WithEmptyIp_ReturnsBadRequest()
    {
        // Arrange
        var ip = "";
        var page = 1;
        var pageSize = 10;

        // Act
        var result = await _controller.FindUsersByIp(ip, page, pageSize);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorArray = Assert.IsType<object[]>(badRequestResult.Value);
        Assert.Equal("IP is required", errorArray[0]);
    }

    [Fact]
    public async Task FindUsersByIp_WithInvalidIp_ReturnsBadRequest()
    {
        // Arrange
        var ip = "invalid";
        var page = 1;
        var pageSize = 10;
        
        _mediator.When(x => x.Send(Arg.Any<FindUsersByIpQuery>(), Arg.Any<CancellationToken>()))
            .Do(x => { throw new ArgumentException("Invalid IP format"); });

        // Act
        var result = await _controller.FindUsersByIp(ip, page, pageSize);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorArray = Assert.IsType<object[]>(badRequestResult.Value);
        Assert.Equal("Invalid IP format", errorArray[0]);
    }
} 