using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Categories.Dtos;

public sealed class CategoryTagResponseDto : ResponseDtoBase<CategoryTag>, IFromEntity<CategoryTag, CategoryTagResponseDto>
{
    private CategoryTagResponseDto(CategoryTag entity)
        : base(entity)
    {
        TagText = entity.TagText;
        SortOrder = entity.SortOrder;
    }

    public string TagText { get; init; }
    public int SortOrder { get; init; }

    public static CategoryTagResponseDto FromEntity(CategoryTag entity)
        => new(entity);
}