using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Categories.Dtos;

public sealed class UpdateCategoryRequestDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Color { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public List<CategoryTagRequestDto> Tags { get; set; } = [];
}