using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Profiles.Dtos;

public sealed class CreateProfileRequestDto
{
    [Required]
    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? WebsiteName { get; set; }

    [MaxLength(512)]
    public string? DefaultTargets { get; set; }
}
