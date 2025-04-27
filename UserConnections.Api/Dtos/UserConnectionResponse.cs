namespace UserConnections.Api.Dtos;

public class UserConnectionResponse
{
    public long UserId { get; set; }
    public DateTime LastConnectionUtc { get; set; }
} 