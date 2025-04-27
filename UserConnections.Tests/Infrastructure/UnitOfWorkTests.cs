using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserConnections.Infrastructure.Persistence;
using UserConnections.Infrastructure.Repositories;

namespace UserConnections.Tests.Infrastructure;

public class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_CallsContextSaveChanges()
    {
        // Arrange
        var mockContext = Substitute.For<UserConnectionDbContext>(new DbContextOptions<UserConnectionDbContext>());
        var unitOfWork = new UnitOfWork(mockContext);
        
        mockContext.SaveChangesAsync(default).ReturnsForAnyArgs(1);
        
        // Act
        await unitOfWork.SaveChangesAsync();
        
        // Assert
        await mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
} 