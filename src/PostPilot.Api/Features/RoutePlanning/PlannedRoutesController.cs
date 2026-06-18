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
    [HttpGet("categories")]
    [HttpPost("categories")]
    [HttpPost("media")]
    [HttpPost("posts/preview")]
    [HttpPost("posts")]
    [HttpGet("queue")]
    [HttpPut("queue")]
    [HttpPost("queue/post-next")]
    public IActionResult PlannedProfileRoute(Guid id)
        => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPut("categories/{catId:guid}")]
    [HttpDelete("categories/{catId:guid}")]
    public IActionResult PlannedCategoryRoute(Guid id, Guid catId)
        => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPost("posts/{postId:guid}/publish")]
    [HttpPost("posts/{postId:guid}/queue")]
    public IActionResult PlannedPostRoute(Guid id, Guid postId)
        => StatusCode(StatusCodes.Status501NotImplemented);
}