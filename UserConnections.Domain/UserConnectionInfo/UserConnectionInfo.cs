namespace UserConnections.Domain.UserConnectionInfo;

public class UserConnectionInfo
{
    public long UserId { get; }
    
    public string IpAddress { get; }
    
    public DateTime LastConnectionUtc { get; }
    
    private UserConnectionInfo(long userId, string ipAddress, DateTime lastConnectionUtc)
    {
        UserId = userId;
        IpAddress = ipAddress;
        LastConnectionUtc = lastConnectionUtc;
    }
    
    public static UserConnectionInfo Create(long userId, string ipAddress, DateTime lastConnectionUtc)
    {
        if (userId < 1)
            throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be greater than 0.");

        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty.", nameof(ipAddress));

        if (lastConnectionUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Last connection time must be in UTC.", nameof(lastConnectionUtc));

        return new UserConnectionInfo(userId, ipAddress, lastConnectionUtc);
    }
}