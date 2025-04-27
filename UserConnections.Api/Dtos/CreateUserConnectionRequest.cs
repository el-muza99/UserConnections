namespace UserConnections.Api.Models;

public class CreateUserConnectionRequest
{
    public long UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
} 