using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.History.Dtos;
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
    public async Task<ActionResult<IReadOnlyCollection<PostHistoryResponseDto>>> List(
        Guid profileId,
        [FromServices] HistoryQueryExecutor query,
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
}