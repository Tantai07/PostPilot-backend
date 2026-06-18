using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Shared;

namespace PostPilot.Api.Features.RoutePlanning;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{id:guid}")]
public sealed class PlannedRoutesController : ControllerBase
{
    [HttpGet("dashboard")]
    [HttpPost("media")]
    [HttpPost("posts/preview")]
    [HttpPost("posts")]
    [HttpGet("queue")]
    [HttpPut("queue")]
    [HttpPost("queue/post-next")]
    [HttpGet("history")]
    public IActionResult PlannedProfileRoute(Guid id)
        => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPost("posts/{postId:guid}/publish")]
    [HttpPost("posts/{postId:guid}/queue")]
    public IActionResult PlannedPostRoute(Guid id, Guid postId)
        => StatusCode(StatusCodes.Status501NotImplemented);
}