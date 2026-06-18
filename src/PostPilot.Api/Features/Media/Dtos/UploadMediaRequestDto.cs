using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Media.Dtos;

public sealed class UploadMediaRequestDto
{
    [Required]
    public IFormFile? File { get; set; }

    [Range(1, int.MaxValue)]
    public int SortOrder { get; set; } = 1;
}