using MediatR;
using UserConnections.Application.Repositories;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Application.Handlers;

public record GetLastConnectionByIpQuery(string Ip) : IRequest<UserConnectionInfo?>;

public class GetLastConnectionByIpHandler : IRequestHandler<GetLastConnectionByIpQuery, UserConnectionInfo?>
{
    private readonly IUserConnectionRepository _repository;

    public GetLastConnectionByIpHandler(IUserConnectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserConnectionInfo?> Handle(GetLastConnectionByIpQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Ip))
        {
            throw new ArgumentException("IP is required", nameof(request.Ip));
        }

        return await _repository.GetLastConnectionByIpAsync(request.Ip, cancellationToken);
    }
} 