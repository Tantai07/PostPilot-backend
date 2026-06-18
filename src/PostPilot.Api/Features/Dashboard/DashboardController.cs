using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Dashboard.Dtos;
using PostPilot.Api.Features.Dashboard.Queries;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Dashboard;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/dashboard")]
public sealed class DashboardController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DashboardResponseDto>> Get(
        Guid profileId,
        [FromServices] DashboardQueryExecutor query,
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