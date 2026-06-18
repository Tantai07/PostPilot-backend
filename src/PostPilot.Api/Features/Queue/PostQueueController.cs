using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Queue.Commands;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Queue;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/posts/{postId:guid}/queue")]
public sealed class PostQueueController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QueueItemResponseDto>> AddPostToQueue(
        Guid profileId,
        Guid postId,
        [FromBody] AddPostToQueueRequestDto? request,
        [FromServices] AddPostToQueueCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(
            currentUser.UserId.Value,
            profileId,
            postId,
            request?.ScheduledAt,
            cancellationToken);

        return response is null ? NotFound() : CreatedAtAction(nameof(AddPostToQueue), new { profileId, postId }, response);
    }
}