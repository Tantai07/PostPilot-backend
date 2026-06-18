using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Media.Commands;
using PostPilot.Api.Features.Media.Dtos;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Media;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/profiles/{profileId:guid}/media")]
public sealed class MediaController : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MediaUploadResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaUploadResponseDto>> Upload(
        Guid profileId,
        [FromForm] UploadMediaRequestDto request,
        [FromServices] UploadMediaCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        if (!MediaUploadValidator.TryValidate(request.File, out var errorMessage))
        {
            ModelState.AddModelError(nameof(request.File), errorMessage ?? "Invalid image file.");
            return ValidationProblem(ModelState);
        }

        var publicBaseUri = new Uri($"{Request.Scheme}://{Request.Host.ToUriComponent()}/");
        var response = await command.ExecuteAsync(
            currentUser.UserId.Value,
            profileId,
            request,
            publicBaseUri,
            cancellationToken);

        return response is null ? NotFound() : Created(response.PublicUrl, response);
    }
}