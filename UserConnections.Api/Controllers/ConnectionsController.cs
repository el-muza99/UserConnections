using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserConnections.Api.Dtos;
using UserConnections.Api.Models;
using UserConnections.Application.Handlers;

namespace UserConnections.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConnectionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Records a new user connection
    /// </summary>
    /// <param name="request">The connection details</param>
    /// <param name="ct">Cancellation token</param>
    [HttpPost]
    public async Task<IActionResult> CreateConnection([FromBody] CreateUserConnectionRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.IpAddress))
        {
            return BadRequest(new { Error = "IP address is required" });
        }

        var command = new CreateUserConnection(request.UserId, request.IpAddress);
        await mediator.Send(command, ct);

        return Ok();
    }

    /// <summary>
    /// Gets all IP addresses for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of IP addresses and their last connection times</returns>
    /// <response code="200">Returns the list of IP addresses</response>
    /// <response code="400">If the user ID is invalid</response>
    /// <response code="404">If no connections found for the user</response>
    [HttpGet("{userId}/ips")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserIpsResponse>> GetUserIps(
        [FromRoute] long userId,
        CancellationToken ct = default)
    {
        if (userId <= 0)
        {
            return BadRequest(new { Error = "User ID must be greater than 0" });
        }

        try
        {
            var query = new GetUserIpsQuery(userId);
            var result = await mediator.Send(query, ct);

            if (!result.Connections.Any())
            {
                return NotFound(new { Error = $"No connections found for user {userId}" });
            }

            return Ok(new UserIpsResponse
            {
                UserId = result.UserId,
                Connections = result.Connections.Select(c => new IpConnectionInfo
                {
                    IpAddress = c.IpAddress,
                    LastConnectionUtc = c.LastConnectionUtc
                }).ToList()
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the last connection for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Last connection details</returns>
    /// <response code="200">Returns the last connection details</response>
    /// <response code="400">If the user ID is invalid</response>
    /// <response code="404">If no connections found for the user</response>
    [HttpGet("{userId}/last")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LastConnectionResponse>> GetLastConnection(
        [FromRoute] long userId,
        CancellationToken ct = default)
    {
        if (userId <= 0)
        {
            return BadRequest(new { Error = "User ID must be greater than 0" });
        }

        try
        {
            var query = new GetLastUserConnectionQuery(userId);
            var lastConnection = await mediator.Send(query, ct);

            if (lastConnection == null)
            {
                return NotFound(new { Error = $"No connections found for user {userId}" });
            }

            return Ok(new LastConnectionResponse
            {
                UserId = userId,
                IpAddress = lastConnection.IpAddress,
                LastConnectionUtc = lastConnection.LastConnectionUtc
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

