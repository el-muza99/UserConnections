using Microsoft.EntityFrameworkCore;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Aggregates;
using UserConnections.Domain.UserConnectionInfo;
using UserConnections.Infrastructure.Entities;
using UserConnections.Infrastructure.Persistence;

namespace UserConnections.Infrastructure.Repositories;

public class UserConnectionRepository : IUserConnectionRepository
{
    private readonly UserConnectionDbContext _dbContext;

    public UserConnectionRepository(UserConnectionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UpsertAsync(UserConnection userConnection, CancellationToken ct = default)
    {
        var entity = await _dbContext.UserConnections
            .FirstOrDefaultAsync(x => 
                x.UserId == userConnection.UserId && 
                x.IpAddress == userConnection.IpAddress.Value, 
                ct);

        if (entity == null)
        {
            entity = new UserConnectionEntity
            {
                UserId = userConnection.UserId,
                IpAddress = userConnection.IpAddress.Value,
                LastConnectionUtc = userConnection.LastConnectionUtc
            };
            
            _dbContext.UserConnections.Add(entity);
            await _dbContext.SaveChangesAsync(ct);
            return true;
        }
        
        if (entity.LastConnectionUtc >= userConnection.LastConnectionUtc)
        {
            return false;
        }

        entity.LastConnectionUtc = userConnection.LastConnectionUtc;
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<(IEnumerable<long> UserIds, int TotalCount)> FindUsersByIpPrefixAsync(
        string ipPrefix, 
        int page = 1, 
        int pageSize = 100, 
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Min(1000, Math.Max(1, pageSize));
        
        var query = _dbContext.UserConnections
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
        var connections = await _dbContext.UserConnections
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
        var lastConnection = await _dbContext.UserConnections
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LastConnectionUtc)
            .FirstOrDefaultAsync(ct);
            
        if (lastConnection == null)
            return null;
            
        return Domain.UserConnectionInfo.UserConnectionInfo.Create(
            lastConnection.UserId,
            lastConnection.IpAddress,
            lastConnection.LastConnectionUtc);
    }

    public async Task<UserConnectionInfo?> GetLastConnectionByIpAsync(
        string ip, 
        CancellationToken ct = default)
    {
        var lastConnection = await _dbContext.UserConnections
            .Where(x => x.IpAddress == ip)
            .OrderByDescending(x => x.LastConnectionUtc)
            .FirstOrDefaultAsync(ct);
            
        if (lastConnection == null)
            return null;
            
        return Domain.UserConnectionInfo.UserConnectionInfo.Create(
            lastConnection.UserId,
            lastConnection.IpAddress,
            lastConnection.LastConnectionUtc);
    }
} 