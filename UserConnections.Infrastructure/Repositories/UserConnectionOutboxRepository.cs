using System.Net;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Events;
using UserConnections.Infrastructure.Entities;
using UserConnections.Infrastructure.Persistence;

namespace UserConnections.Infrastructure.Repositories;

public class UserConnectionOutboxRepository : IUserConnectionOutboxRepository
{
    private readonly UserConnectionDbContext _dbContext;

    public UserConnectionOutboxRepository(UserConnectionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(ConnectionEvent connectionEvent, CancellationToken cancellationToken = default)
    {
        var outboxEvent = new ConnectionEventOutbox
        {
            Id = Guid.NewGuid(),
            UserId = connectionEvent.UserId,
            IpAddress = connectionEvent.IpAddress.Value,
            ConnectionTimeUtc = connectionEvent.ConnectedAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        };
        
        _dbContext.ConnectionEvents.Add(outboxEvent);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
} 