using Microsoft.AspNetCore.Mvc;

namespace PostPilot.Api.Features.Media.Dtos;

public sealed class MediaUploadRequestDto
{
    [FromForm(Name = "file")]
    public IFormFile? File { get; init; }
}
