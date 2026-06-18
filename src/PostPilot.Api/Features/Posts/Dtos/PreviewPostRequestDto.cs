using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PreviewPostRequestDto
{
    public Guid? CategoryId { get; set; }

    [Required]
    [MaxLength(4096)]
    public string Caption { get; set; } = string.Empty;

    public List<Guid> MediaIds { get; set; } = [];

    public List<string> TargetPlatforms { get; set; } = [];
}