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
public sealed class CategoriesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> Create(
        Guid profileId,
        [FromBody] CreateCategoryRequestDto request,
        [FromServices] CreateCategoryCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await command.ExecuteAsync(currentUser.UserId.Value, profileId, request, cancellationToken);
        return response is null
            ? NotFound()
            : Created($"/api/profiles/{profileId}/categories/{response.Id}", response);
    }

    [HttpPut("{categoryId:guid}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> Update(
        Guid profileId,
        Guid categoryId,
        [FromBody] UpdateCategoryRequestDto request,
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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