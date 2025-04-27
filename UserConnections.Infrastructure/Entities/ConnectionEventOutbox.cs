namespace UserConnections.Infrastructure.Entities;

public class ConnectionEventOutbox
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public string IpAddress { get; set; } = null!;
    public DateTime ConnectionTimeUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
} 