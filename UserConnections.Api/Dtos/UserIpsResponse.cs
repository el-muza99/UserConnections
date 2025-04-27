namespace UserConnections.Api.Dtos;

public class UserIpsResponse
{
    public long UserId { get; set; }
    public List<IpConnectionInfo> Connections { get; set; } = new();
}

public class IpConnectionInfo
{
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LastConnectionUtc { get; set; }
} 