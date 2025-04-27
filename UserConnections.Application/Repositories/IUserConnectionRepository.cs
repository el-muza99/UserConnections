using UserConnections.Domain.Aggregates;
using UserConnections.Domain.UserConnectionInfo;
using UserConnections.Domain.ValueObjects;

namespace UserConnections.Application.Repositories;

public interface IUserConnectionRepository
{
    /// <summary>
    /// Upserts a single user connection
    /// </summary>
    /// <param name="userConnections">User connection to upsert</param>
    /// <param name="ct">Cancellation token</param>
    Task UpsertAsync(
        IEnumerable<UserConnection> userConnections,
        CancellationToken ct = default);
    
    /// <summary>
    /// Finds user IDs by partial IP address match
    /// </summary>
    /// <param name="ipPrefix">IP address prefix to search for</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size (max 1000)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of user IDs and total count</returns>
    Task<(IEnumerable<long> UserIds, int TotalCount)> FindUsersByIpPrefixAsync(
        string ipPrefix,
        int page = 1,
        int pageSize = 100,
        CancellationToken ct = default);
        
    /// <summary>
    /// Gets all IP addresses for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>IP addresses and last connection times</returns>
    Task<IEnumerable<UserConnectionInfo>> GetUserIpsAsync(
        long userId,
        CancellationToken ct = default);
        
    /// <summary>
    /// Gets the last connection for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Last connection info or null if not found</returns>
    Task<UserConnectionInfo?> GetLastUserConnectionAsync(
        long userId,
        CancellationToken ct = default);
        
    /// <summary>
    /// Gets the last connection from a specific IP address
    /// </summary>
    /// <param name="ip">IP address</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Last connection info or null if not found</returns>
    Task<UserConnectionInfo?> GetLastConnectionByIpAsync(
        string ip,
        CancellationToken ct = default);
}