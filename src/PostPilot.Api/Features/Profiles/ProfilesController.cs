using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Profiles.Dtos;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Profiles;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles")]
public sealed class ProfilesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> List(
        [FromQuery] ProfileQuery query,
        [FromServices] ProfileQueryExecutor executor,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        return Ok(await executor.ExecuteAsync(currentUser.UserId.Value, query, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProfileResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProfileResponseDto>> Create(
        [FromBody] CreateProfileRequestDto request,
        [FromServices] CreateProfileCommand command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(List), new { id = response.Id }, response);
    }
}
