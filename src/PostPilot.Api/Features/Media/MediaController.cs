using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostPilot.Api.Features.Media.Commands;
using PostPilot.Api.Features.Media.Dtos;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Features.Media;

[ApiController]
[Authorize]
[Route("api/profiles/{profileId:guid}/media")]
public sealed class MediaController : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(105_906_176)]
    [RequestFormLimits(MultipartBodyLengthLimit = 105_906_176)]
    [ProducesResponseType(typeof(MediaUploadResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaUploadResponseDto>> Upload(
        Guid profileId,
        [FromForm] MediaUploadRequestDto request,
        [FromServices] UploadMediaCommandExecutor command,
        [FromServices] ICurrentUserContext currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        if (request.File is null)
        {
            return BadRequest("Image file is required.");
        }

        try
        {
            var response = await command.ExecuteAsync(
                currentUser.UserId.Value,
                profileId,
                request.File,
                Request,
                cancellationToken);

            return response is null ? NotFound() : Created(response.PublicUrl, response);
        }
        catch (InvalidOperationException error)
        {
            return BadRequest(error.Message);
        }
    }
}
