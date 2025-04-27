using UserConnections.Domain.Events;

namespace UserConnections.Application.Repositories;

public interface IUserConnectionOutboxRepository
{
    /// <summary>
    /// Saves a connection event into the outbox storage.
    /// </summary>
    /// <param name="connectionEvent">The connection event to store</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveAsync(ConnectionEvent connectionEvent, CancellationToken cancellationToken = default);
}