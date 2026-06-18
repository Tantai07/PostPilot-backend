using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PreviewPostRequestDto
{
    [Required]
    [MaxLength(4096)]
    public string Caption { get; set; } = string.Empty;

    public Guid? CategoryId { get; set; }

    public List<PostMediaRequestDto> Media { get; set; } = [];

    public List<PostTargetRequestDto> Targets { get; set; } = [];
}