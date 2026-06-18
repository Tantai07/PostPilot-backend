using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostTargetRequestDto
{
    [Required]
    [MaxLength(32)]
    public string TargetPlatform { get; set; } = string.Empty;

    [Required]
    public Guid TargetAccountId { get; set; }
}