using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Meta.Dtos;

public sealed class MetaConnectionRequestDto
{
    [Required]
    [MaxLength(128)]
    public string FacebookPageId { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string FacebookPageName { get; set; } = string.Empty;

    [Required]
    [MaxLength(4096)]
    public string PageAccessToken { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    [MaxLength(128)]
    public string? InstagramBusinessAccountId { get; set; }

    [MaxLength(160)]
    public string? InstagramDisplayName { get; set; }
}
