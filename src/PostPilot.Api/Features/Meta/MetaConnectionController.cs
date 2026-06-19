using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Meta.Commands;
using PostPilot.Api.Features.Meta.Dtos;
using PostPilot.Api.Features.Meta.Queries;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Meta;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/meta-connection")]
public sealed class MetaConnectionController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(MetaConnectionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MetaConnectionResponseDto>> Get(
        Guid profileId,
        [FromServices] MetaConnectionQueryExecutor query,
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
    [ProducesResponseType(typeof(MetaConnectionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MetaConnectionResponseDto>> Save(
        Guid profileId,
        [FromBody] MetaConnectionRequestDto request,
        [FromServices] SaveMetaConnectionCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.FacebookPageId)
            || string.IsNullOrWhiteSpace(request.FacebookPageName)
            || string.IsNullOrWhiteSpace(request.PageAccessToken))
        {
            return BadRequest("Facebook page id, page name, and page access token are required.");
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, request, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }
}
