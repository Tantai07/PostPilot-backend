using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Publishing.Commands;
using PostPilot.Api.Features.Publishing.Dtos;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Publishing;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/posts/{postId:guid}/publish")]
public sealed class PublishingController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PublishPostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublishPostResponseDto>> PublishNow(
        Guid profileId,
        Guid postId,
        [FromServices] PublishPostCommandExecutor command,
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