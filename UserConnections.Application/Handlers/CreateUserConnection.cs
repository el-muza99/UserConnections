using MediatR;
using UserConnections.Application.Repositories;
using UserConnections.Domain.Events;
using UserConnections.Domain.ValueObjects;

namespace UserConnections.Application.Handlers;

public record CreateUserConnection(long UserId, string IpAddress) : IRequest;


public class CreateUserConnectionHandler(
    IUserConnectionOutboxRepository outboxRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserConnection>
{
    public async Task Handle(CreateUserConnection request, CancellationToken cancellationToken)
    {
        var ipAddress = IpAddress.Create(request.IpAddress);
        var connectedAtUtc = DateTime.UtcNow;
        
        var connectionEvent = ConnectionEvent.Create(
            request.UserId,
            ipAddress,
            connectedAtUtc);
        
        await outboxRepository.SaveAsync(connectionEvent, cancellationToken);
        
        // in one transaction
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

