using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserConnections.Api.Dtos;
using UserConnections.Api.Models;
using UserConnections.Application.Handlers;
using UserConnections.Domain.UserConnectionInfo;

namespace UserConnections.Api.Controllers;

/// <summary>
/// Controller for querying IP addresses
/// </summary>
[ApiController]
[Route("[controller]")]
public class IpsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IpsController> _logger;

    public IpsController(
        IMediator mediator,
        ILogger<IpsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Find users by IP address 
    /// </summary>
    /// <param name="ip">IP address to search for</param>
    /// <param name="page">Page number (1-based, default: 1)</param>
    /// <param name="pageSize">Page size (default: 100, max: 1000)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of user IDs that have connected from matching IPs</returns>
    /// <response code="200">Returns the list of user IDs</response>
    /// <response code="400">If the IP is invalid or pagination parameters are out of range</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FindUsersByIp(
        [FromQuery] string ip,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            return BadRequest(new object[] { "IP is required" });
        }

        try
        {
            var query = new FindUsersByIpQuery(ip, page, pageSize);
            var result = await _mediator.Send(query, ct);

            return Ok(new UsersSearchResponse
            {
                UserIds = result.UserIds.ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new object[] { ex.Message });
        }
    }
    
    /// <summary>
    /// Get the most recent user connection from a specific IP address
    /// </summary>
    /// <param name="ip">IP address</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>User ID and timestamp of the most recent connection</returns>
    /// <response code="200">Returns the connection details</response>
    /// <response code="400">If the IP address is invalid</response>
    /// <response code="404">If no connections found for the IP</response>
    [HttpGet("{ip}/last")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLastConnectionByIp(
        [FromRoute] string ip,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            return BadRequest(new object[] { "IP is required" });
        }

        try
        {
            var query = new GetLastConnectionByIpQuery(ip);
            var connection = await _mediator.Send(query, ct);

            if (connection == null)
            {
                return NotFound(new object[] { $"No connections found for IP {ip}" });
            }

            return Ok(new UserConnectionResponse
            {
                UserId = connection.UserId,
                LastConnectionUtc = connection.LastConnectionUtc
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new object[] { ex.Message });
        }
    }
}

