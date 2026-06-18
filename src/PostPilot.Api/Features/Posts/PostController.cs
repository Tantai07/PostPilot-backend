using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Posts.Commands;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Posts;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/posts")]
public sealed class PostController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostResponseDto>> CreateDraft(
        Guid profileId,
        [FromBody] CreatePostDraftRequestDto request,
        [FromServices] CreatePostDraftCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Caption))
        {
            return BadRequest("Caption is required.");
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, request, cancellationToken);
        return response is null ? NotFound() : CreatedAtAction(nameof(CreateDraft), new { profileId }, response);
    }
}