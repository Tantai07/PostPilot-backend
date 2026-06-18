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
        UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt;
        Tags = entity.Tags
            .Where(x => x.DeletedAt is null)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(CategoryTagResponseDto.FromEntity)
            .ToList();
    }

    public Guid ProfileId { get; init; }
    public string Name { get; init; }
    public string Color { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public IReadOnlyList<CategoryTagResponseDto> Tags { get; init; }

    public static CategoryResponseDto FromEntity(Category entity)
        => new(entity);
}