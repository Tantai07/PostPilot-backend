using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Categories.Dtos;

public sealed class CategoryRequestDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(32)]
    public string? Color { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? CaptionTemplate { get; set; }

    public List<string> Tags { get; set; } = [];
}