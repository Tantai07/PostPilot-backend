using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Queue.Commands;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Api.Features.Queue.Queries;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Queue;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/queue")]
public sealed class QueueController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<QueueItemResponseDto>>> List(
        Guid profileId,
        [FromServices] QueueQueryExecutor query,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await query.ExecuteAsync(currentUser.UserId.Value, profileId, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<IReadOnlyCollection<QueueItemResponseDto>>> Reorder(
        Guid profileId,
        [FromBody] ReorderQueueRequestDto request,
        [FromServices] ReorderQueueCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, request, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("post-next")]
    public async Task<ActionResult<QueueItemResponseDto>> PostNext(
        Guid profileId,
        [FromServices] PostNextQueueCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }
}