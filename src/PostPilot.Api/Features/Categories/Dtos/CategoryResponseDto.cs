using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Categories.Dtos;

public sealed class CategoryResponseDto : ResponseDtoBase<Category>, IFromEntity<Category, CategoryResponseDto>
{
    private CategoryResponseDto(Category entity)
        : base(entity)
    {
        ProfileId = entity.ProfileId;
        Name = entity.Name;
        Color = entity.Color;
        Description = entity.Description;
        CaptionTemplate = entity.CaptionTemplate;
        Tags = entity.Tags
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => x.TagText)
            .ToList();
        UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt;
    }

    public Guid ProfileId { get; init; }
    public string Name { get; init; }
    public string Color { get; init; }
    public string? Description { get; init; }
    public string? CaptionTemplate { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public static CategoryResponseDto FromEntity(Category entity)
        => new(entity);
}