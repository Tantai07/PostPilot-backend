using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Posts.Commands;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Api.Features.Posts.Queries;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Posts;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/posts")]
public sealed class PostController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PostResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<PostResponseDto>>> List(
        Guid profileId,
        [FromQuery] PostListQuery query,
        [FromServices] PostListQueryExecutor executor,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await executor.ExecuteAsync(currentUser.UserId.Value, profileId, query, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

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

    [HttpPost("{postId:guid}/publish-now")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostResponseDto>> PublishNow(
        Guid profileId,
        Guid postId,
        [FromServices] PublishPostNowCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, postId, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }
}