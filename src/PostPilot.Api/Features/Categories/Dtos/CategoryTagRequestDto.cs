using System.ComponentModel.DataAnnotations;

namespace PostPilot.Api.Features.Categories.Dtos;

public sealed class CategoryTagRequestDto
{
    [Required]
    [MaxLength(120)]
    public string TagText { get; set; } = string.Empty;

    public int? SortOrder { get; set; }
}