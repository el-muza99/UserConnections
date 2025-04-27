using NSubstitute;
using UserConnections.Application.Handlers;
using UserConnections.Application.Repositories;
using UserConnections.Domain.ValueObjects;

namespace UserConnections.Tests.Application;

public class FindUsersByIpPrefixHandlerTests
{
    private readonly IUserConnectionRepository _mockRepository;
    private readonly FindUsersByIpHandler _handler;

    public FindUsersByIpPrefixHandlerTests()
    {
        _mockRepository = Substitute.For<IUserConnectionRepository>();
        _handler = new FindUsersByIpHandler(_mockRepository);
    }

    [Fact]
    public async Task Handle_WithValidIpPrefix_ReturnsMatchingUsers()
    {
        // Arrange
        var ipPrefix = "192.168";
        var page = 1;
        var pageSize = 100;
        var query = new FindUsersByIpQuery(ipPrefix, page, pageSize);
        
        var matchingUserIds = new List<long> { 1, 2, 3 };
        _mockRepository.FindUsersByIpPrefixAsync(ipPrefix, page, pageSize, default)
            .Returns((matchingUserIds, matchingUserIds.Count));
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        Assert.Equal(matchingUserIds, result.UserIds);
        Assert.Equal(matchingUserIds.Count, result.TotalCount);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
    }

    [Fact]
    public async Task Handle_WithInvalidIpPrefix_ThrowsArgumentException()
    {
        // Arrange
        var invalidIpPrefix = "";
        var query = new FindUsersByIpQuery(invalidIpPrefix, 1, 100);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(query, CancellationToken.None));
    }
} 