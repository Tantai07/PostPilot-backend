using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Categories.Commands;
using PostPilot.Api.Features.Categories.Dtos;
using PostPilot.Api.Features.Categories.Queries;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Categories;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/categories")]
public sealed class CategoryEndpoints : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<object>> List(
        Guid profileId,
        [FromQuery] CategoryQuery query,
        [FromServices] CategoryQueryExecutor executor,
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
    public async Task<ActionResult<CategoryResponseDto>> Create(
        Guid profileId,
        [FromBody] CategoryRequestDto request,
        [FromServices] CreateCategoryCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, request, cancellationToken);
        return response is null ? NotFound() : CreatedAtAction(nameof(List), new { profileId }, response);
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<ActionResult<CategoryResponseDto>> Update(
        Guid profileId,
        Guid categoryId,
        [FromBody] CategoryRequestDto request,
        [FromServices] UpdateCategoryCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, categoryId, request, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Delete(
        Guid profileId,
        Guid categoryId,
        [FromServices] DeleteCategoryCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var deleted = await command.ExecuteAsync(currentUser.UserId.Value, profileId, categoryId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}