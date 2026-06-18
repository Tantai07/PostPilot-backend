using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostMediaRequestDto
{
    [Required]
    [MaxLength(2048)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(2048)]
    public string? PublicUrl { get; set; }

    [MaxLength(32)]
    public string StorageProvider { get; set; } = "Local";

    [Range(1, int.MaxValue)]
    public int? SortOrder { get; set; }
}