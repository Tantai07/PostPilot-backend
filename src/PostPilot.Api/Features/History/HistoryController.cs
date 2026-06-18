using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.History.Queries;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.History;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/history")]
public sealed class HistoryController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> List(
        Guid profileId,
        [FromQuery] HistoryQuery request,
        [FromServices] HistoryQueryExecutor query,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await query.ExecuteAsync(currentUser.UserId.Value, profileId, request, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }
}