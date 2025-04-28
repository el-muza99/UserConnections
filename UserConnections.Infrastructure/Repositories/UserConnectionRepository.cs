using Microsoft.EntityFrameworkCore;
using System.Net;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Aggregates;
using UserConnections.Domain.UserConnectionInfo;
using UserConnections.Infrastructure.Entities;
using UserConnections.Infrastructure.Persistence;

namespace UserConnections.Infrastructure.Repositories;

public class UserConnectionRepository(UserConnectionDbContext dbContext) : IUserConnectionRepository
{
    public async Task UpsertAsync(
        IEnumerable<UserConnection> userConnections,
        CancellationToken ct = default)
    {
        var connections = userConnections.ToList();
        if (!connections.Any()) return;
        
        var (userIds, ipAddresses) = ExtractUniqueIdentifiers(connections);
        var existingEntities = await FetchExistingConnections(userIds, ipAddresses, ct);
        var existingConnectionsMap = BuildConnectionsMap(existingEntities);
        var newConnections = ProcessConnections(connections, existingConnectionsMap);
        
        await SaveChanges(newConnections, ct);
    }

    public async Task<(IEnumerable<long> UserIds, int TotalCount)> FindUsersByIpPrefixAsync(
        string ipPrefix, 
        int page = 1, 
        int pageSize = 100, 
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Min(1000, Math.Max(1, pageSize));
        
        var query = dbContext
            .UserConnections
            .AsNoTracking()
            .Where(x => EF.Functions.ILike(x.IpAddress, $"{ipPrefix}%"))
            .Select(x => x.UserId)
            .Distinct();
            
        var totalCount = await query.CountAsync(ct);
        var userIds = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
            
        return (userIds, totalCount);
    }

    public async Task<IEnumerable<UserConnectionInfo>> GetUserIpsAsync(
        long userId, 
        CancellationToken ct = default)
    {
        var connections = await dbContext
            .UserConnections
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);
            
        return connections.Select(x => 
            Domain.UserConnectionInfo.UserConnectionInfo.Create(
                x.UserId, 
                x.IpAddress, 
                x.LastConnectionUtc));
    }

    public async Task<UserConnectionInfo?> GetLastUserConnectionAsync(
        long userId, 
        CancellationToken ct = default)
    {
        var lastConnection = await dbContext
            .UserConnections
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LastConnectionUtc)
            .FirstOrDefaultAsync(ct);
            
        if (lastConnection == null)
            return null;
            
        return UserConnectionInfo.Create(
            lastConnection.UserId,
            lastConnection.IpAddress,
            lastConnection.LastConnectionUtc);
    }

    public async Task<UserConnectionInfo?> GetLastConnectionByIpAsync(
        string ip, 
        CancellationToken ct = default)
    {
        var lastConnection = await dbContext
            .UserConnections
            .AsNoTracking()
            .Where(x => x.IpAddress == ip)
            .OrderByDescending(x => x.LastConnectionUtc)
            .FirstOrDefaultAsync(ct);
            
        if (lastConnection == null)
            return null;
            
        return UserConnectionInfo.Create(
            lastConnection.UserId,
            lastConnection.IpAddress,
            lastConnection.LastConnectionUtc);
    }
    
        
    private (List<long> UserIds, List<string> IpAddresses) ExtractUniqueIdentifiers(List<UserConnection> connections)
    {
        var userIds = connections.Select(c => c.UserId).Distinct().ToList();
        var ipAddresses = connections.Select(c => c.IpAddress.Value).Distinct().ToList();
        return (userIds, ipAddresses);
    }
    
    private async Task<List<UserConnectionEntity>> FetchExistingConnections(
        List<long> userIds, 
        List<string> ipAddresses, 
        CancellationToken ct)
    {
        return await dbContext.UserConnections
            .Where(x => userIds.Contains(x.UserId) && ipAddresses.Contains(x.IpAddress))
            .ToListAsync(ct);
    }
    
    private Dictionary<(long UserId, string IpAddress), UserConnectionEntity> BuildConnectionsMap(
        List<UserConnectionEntity> existingEntities)
    {
        return existingEntities
            .ToDictionary(
                key => (key.UserId, key.IpAddress),
                value => value);
    }
    
    private List<UserConnectionEntity> ProcessConnections(
        List<UserConnection> connections,
        Dictionary<(long UserId, string IpAddress), UserConnectionEntity> existingConnectionsMap)
    {
        var newConnections = new List<UserConnectionEntity>();
        
        foreach (var userConnection in connections)
        {
            var connectionKey = (userConnection.UserId, userConnection.IpAddress.Value);
            
            if (existingConnectionsMap.TryGetValue(connectionKey, out var existingConnection))
            {
                UpdateConnectionTimeIfNewer(existingConnection, userConnection.LastConnectionUtc);
            }
            else
            {
                newConnections.Add(CreateNewConnectionEntity(userConnection));
            }
        }
        
        return newConnections;
    }
    
    private async Task SaveChanges(List<UserConnectionEntity> newConnections, CancellationToken ct)
    {
        if (newConnections.Any())
        {
            await dbContext.UserConnections.AddRangeAsync(newConnections, ct);
        }
        
        await dbContext.SaveChangesAsync(ct);
    }

    private static void UpdateConnectionTimeIfNewer(UserConnectionEntity entity, DateTime newConnectionTime)
    {
        if (entity.LastConnectionUtc < newConnectionTime)
        {
            entity.LastConnectionUtc = newConnectionTime;
        }
    }

    private static UserConnectionEntity CreateNewConnectionEntity(UserConnection userConnection) =>
        new()
        {
            UserId = userConnection.UserId,
            IpAddress = userConnection.IpAddress.Value,
            LastConnectionUtc = userConnection.LastConnectionUtc
        };
} 