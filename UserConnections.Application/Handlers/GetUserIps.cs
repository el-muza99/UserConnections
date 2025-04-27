using MediatR;
using UserConnections.Application.Repositories;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Application.Handlers;

public record GetUserIpsQuery(long UserId) : IRequest<GetUserIpsResult>;

public class GetUserIpsResult
{
    public long UserId { get; }
    public IEnumerable<UserConnectionInfo> Connections { get; }

    public GetUserIpsResult(long userId, IEnumerable<UserConnectionInfo> connections)
    {
        UserId = userId;
        Connections = connections;
    }
}

public class GetUserIpsHandler : IRequestHandler<GetUserIpsQuery, GetUserIpsResult>
{
    private readonly IUserConnectionRepository _repository;

    public GetUserIpsHandler(IUserConnectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUserIpsResult> Handle(GetUserIpsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            throw new ArgumentException("User ID must be greater than 0", nameof(request.UserId));
        }

        var connections = await _repository.GetUserIpsAsync(request.UserId, cancellationToken);

        return new GetUserIpsResult(request.UserId, connections);
    }
} 