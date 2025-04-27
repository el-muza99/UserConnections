namespace UserConnections.Api.Dtos;

public class UsersSearchResponse
{
    public List<long> UserIds { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
} 