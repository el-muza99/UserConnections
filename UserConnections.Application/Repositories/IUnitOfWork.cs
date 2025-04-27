namespace UserConnections.Application.Repositories;

public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes into the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected rows</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}