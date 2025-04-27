using MediatR;
using UserConnections.Application.Repositories;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Application.Handlers;

public record GetLastUserConnectionQuery(long UserId) : IRequest<UserConnectionInfo?>;

public class GetLastUserConnectionHandler : IRequestHandler<GetLastUserConnectionQuery, UserConnectionInfo?>
{
    private readonly IUserConnectionRepository _repository;

    public GetLastUserConnectionHandler(IUserConnectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserConnectionInfo?> Handle(GetLastUserConnectionQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(request.UserId));
        }

        return await _repository.GetLastUserConnectionAsync(request.UserId, cancellationToken);
    }
} 