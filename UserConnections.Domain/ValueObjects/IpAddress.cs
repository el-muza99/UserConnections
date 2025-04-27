using System.Net;
using System.Net.Sockets;

namespace UserConnections.Domain.ValueObjects;

public sealed record IpAddress
{
    public string Value { get; }
    public AddressFamily AddressFamily { get; }

    private IpAddress(string value, AddressFamily family)
    {
        Value = value;
        AddressFamily = family;
    }

    public static IpAddress Create(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("IP address cannot be empty.", nameof(ip));

        if (!IPAddress.TryParse(ip, out var parsedIp))
            throw new ArgumentException("Invalid IP address format.", nameof(ip));

        var addressType = parsedIp.AddressFamily switch
        {
            AddressFamily.InterNetwork => AddressFamily.InterNetwork,
            AddressFamily.InterNetworkV6 => AddressFamily.InterNetworkV6,
            _ => throw new ArgumentException("Unknown IP address family.", nameof(ip))
        };

        return new IpAddress(ip, addressType);
    }

    public override string ToString() => Value;
}

