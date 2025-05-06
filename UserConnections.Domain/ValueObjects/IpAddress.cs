using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace UserConnections.Domain.ValueObjects;

public sealed record IpAddress
{
    private static readonly Regex IPv4Regex = new(@"^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$", RegexOptions.Compiled);
    private static readonly Regex IPv6Regex = new(@"^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^::$|^::1$|^([0-9a-fA-F]{1,4}:){1,7}:$|^:((:[0-9a-fA-F]{1,4}){1,7})?$|^([0-9a-fA-F]{1,4}:){1,6}:([0-9a-fA-F]{1,4}:){0,1}[0-9a-fA-F]{1,4}$|^([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}$|^([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}$|^([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}$|^([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})$", RegexOptions.Compiled);

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

        // Prevent single integers from being treated as IP addresses
        if (int.TryParse(ip, out _))
            throw new ArgumentException("A single number is not a valid IP address format.", nameof(ip));

        if (!IPAddress.TryParse(ip, out var parsedIp))
            throw new ArgumentException("Invalid IP address format.", nameof(ip));

        // For IPv4, ensure it has the proper 4-octet format (x.x.x.x)
        if ((parsedIp.AddressFamily == AddressFamily.InterNetwork && !IPv4Regex.IsMatch(ip)) 
            || !(parsedIp.AddressFamily == AddressFamily.InterNetworkV6 && IPv6Regex.IsMatch(ip)))
            throw new ArgumentException("IPv4 address must be in format: xxx.xxx.xxx.xxx or IPv6", nameof(ip));

        var addressType = parsedIp.AddressFamily switch
        {
            AddressFamily.InterNetwork => AddressFamily.InterNetwork,
            AddressFamily.InterNetworkV6 => AddressFamily.InterNetworkV6,
            _ => throw new ArgumentException("Unknown IP address family.", nameof(ip))
        };

        // Convert to normalized IP address format
        var normalizedIpString = parsedIp.ToString();
        return new IpAddress(normalizedIpString, addressType);
    }

    public string GetCanonicalFormat()
    {
        if (IPAddress.TryParse(Value, out var parsedIp))
        {
            return parsedIp.ToString();
        }
        return Value; // Fallback to original value if parsing fails
    }

    public override string ToString() => Value;
}

