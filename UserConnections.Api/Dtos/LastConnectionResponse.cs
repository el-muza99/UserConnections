namespace UserConnections.Api.Dtos;

public class LastConnectionResponse
{
    public long UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastConnectionUtc { get; set; }
} 