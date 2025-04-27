using MediatR;
using UserConnections.Application.Repositories;

namespace UserConnections.Application.Handlers;

public record FindUsersByIpQuery(
    string Ip,
    int Page,
    int PageSize) : IRequest<FindUsersByIpResult>;

public class FindUsersByIpResult
{
    public IEnumerable<long> UserIds { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    public FindUsersByIpResult(IEnumerable<long> userIds, int totalCount, int page, int pageSize)
    {
        UserIds = userIds;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}

public class FindUsersByIpHandler : IRequestHandler<FindUsersByIpQuery, FindUsersByIpResult>
{
    private readonly IUserConnectionRepository _repository;

    public FindUsersByIpHandler(IUserConnectionRepository repository)
    {
        _repository = repository;
    }

    public async Task<FindUsersByIpResult> Handle(FindUsersByIpQuery request, CancellationToken cancellationToken)
    {
        // Validate parameters
        if (string.IsNullOrWhiteSpace(request.Ip))
        {
            throw new ArgumentException("IP is required", nameof(request.Ip));
        }

        if (request.Page < 1)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(request.Page));
        }

        if (request.PageSize < 1 || request.PageSize > 1000)
        {
            throw new ArgumentException("Page size must be between 1 and 1000", nameof(request.PageSize));
        }

        // Get the data from the repository
        var (userIds, totalCount) = await _repository.FindUsersByIpPrefixAsync(
            request.Ip,
            request.Page,
            request.PageSize,
            cancellationToken);

        // Return the result
        return new FindUsersByIpResult(
            userIds,
            totalCount,
            request.Page,
            request.PageSize);
    }
} 