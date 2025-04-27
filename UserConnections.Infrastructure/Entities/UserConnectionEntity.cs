namespace UserConnections.Infrastructure.Entities;

public class UserConnectionEntity
{
    public long UserId { get; set; }
    public string IpAddress { get; set; } = null!;
    public DateTime LastConnectionUtc { get; set; }
} 