using System.Net;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Events;
using UserConnections.Infrastructure.Entities;
using UserConnections.Infrastructure.Persistence;

namespace UserConnections.Infrastructure.Repositories;

public class UserConnectionOutboxRepository(UserConnectionDbContext dbContext) : IUserConnectionOutboxRepository
{
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
        
        await dbContext.ConnectionEvents.AddAsync(outboxEvent, cancellationToken);
    }
} 