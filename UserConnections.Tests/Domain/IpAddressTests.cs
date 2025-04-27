using System.Net.Sockets;
using System.Net;
using UserConnections.Domain.ValueObjects;
using Xunit;

namespace UserConnections.Tests.Domain;

public class IpAddressTests
{
    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    [InlineData("127.0.0.1")]
    [InlineData("255.255.255.255")]
    public void Create_WithValidIpv4Address_ShouldReturnIpAddressObject(string ipString)
    {
        // Act
        var ipAddress = IpAddress.Create(ipString);
        
        // Assert
        Assert.NotNull(ipAddress);
        Assert.Equal(ipString, ipAddress.Value);
        Assert.Equal(AddressFamily.InterNetwork, ipAddress.AddressFamily);
    }
    
    [Theory]
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
    [InlineData("2001:db8:85a3::8a2e:370:7334")]
    [InlineData("::1")]
    [InlineData("fe80::1")]
    public void Create_WithValidIpv6Address_ShouldReturnIpAddressObject(string ipString)
    {
        // Act
        var ipAddress = IpAddress.Create(ipString);
        
        // Assert
        Assert.NotNull(ipAddress);
        // IPv6 addresses may be normalized by .NET, so verify by parsing
        Assert.True(IPAddress.TryParse(ipString, out var expected));
        Assert.True(IPAddress.TryParse(ipAddress.Value, out var actual));
        Assert.Equal(expected.ToString(), actual.ToString());
        Assert.Equal(AddressFamily.InterNetworkV6, ipAddress.AddressFamily);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyString_ShouldThrowArgumentException(string ipString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => IpAddress.Create(ipString));
        Assert.Contains("IP address cannot be empty", exception.Message);
    }
    
    [Theory]
    [InlineData("not-an-ip-address")]
    [InlineData("http://example.com")]
    [InlineData("192.168.1.a")]
    [InlineData("abc")]
    [InlineData("192.168.1:8080")]
    public void Create_WithInvalidIpFormat_ShouldThrowArgumentException(string ipString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => IpAddress.Create(ipString));
        Assert.Contains("Invalid IP address format", exception.Message);
    }
    
    [Theory]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("0")]
    public void Create_WithSingleInteger_ShouldThrowArgumentException(string ipString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => IpAddress.Create(ipString));
        Assert.Contains("A single number is not a valid IP address format", exception.Message);
    }
    
    [Fact]
    public void Create_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        string? nullIp = null;
        var exception = Assert.Throws<ArgumentException>(() => IpAddress.Create(nullIp!));
        Assert.Contains("IP address cannot be empty", exception.Message);
    }
} 