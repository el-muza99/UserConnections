using UserConnections.Domain.ValueObjects;

namespace UserConnections.Domain.Events;

public sealed record ConnectionEvent(long UserId, IpAddress IpAddress, DateTime ConnectedAtUtc)
{
    public static ConnectionEvent Create(long userId, IpAddress ipAddress, DateTime connectedAtUtc)
    {
        if (userId < 1)
            throw new ArgumentOutOfRangeException(nameof(userId), "Value must be greater than or equal to 1.");
        
        return new ConnectionEvent(userId, ipAddress, connectedAtUtc);
    }
}