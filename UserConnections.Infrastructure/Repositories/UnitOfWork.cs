using UserConnections.Application.Repositories;
using UserConnections.Infrastructure.Persistence;

namespace UserConnections.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly UserConnectionDbContext _dbContext;

    public UnitOfWork(UserConnectionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
} 