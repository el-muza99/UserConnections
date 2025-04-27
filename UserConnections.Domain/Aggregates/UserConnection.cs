using UserConnections.Domain.ValueObjects;

namespace UserConnections.Domain.Aggregates;

public class UserConnection
{
    public long UserId { get; }
    
    public IpAddress IpAddress { get; }
    
    public DateTime LastConnectionUtc { get; private set; }
    
    private UserConnection(long userId, IpAddress ipAddress, DateTime lastConnectionUtc)
    {
        UserId = userId;
        IpAddress = ipAddress;
        LastConnectionUtc = lastConnectionUtc;
    }
    

    public static UserConnection Create(long userId, IpAddress ipAddress, DateTime connectionTimeUtc)
    {
        if (connectionTimeUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Connection time must be in UTC", nameof(connectionTimeUtc));
        }
        
        return new UserConnection(userId, ipAddress, connectionTimeUtc);
    }
    
    /// <summary>
    /// Updates the last connection timestamp if the new timestamp is more recent
    /// </summary>
    /// <param name="connectionTimeUtc">New connection timestamp (in UTC)</param>
    /// <returns>True if the timestamp was updated, false if it was equal or older</returns>
    /// <exception cref="ArgumentException">Thrown when the timestamp is not in UTC</exception>
    public bool UpdateLastConnection(DateTime connectionTimeUtc)
    {
        if (connectionTimeUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Connection time must be in UTC", nameof(connectionTimeUtc));
        }
        
        if (connectionTimeUtc <= LastConnectionUtc)
        {
            return false;
        }
        
        LastConnectionUtc = connectionTimeUtc;
        return true;
    }
}