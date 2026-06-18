using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostResponseDto : ResponseDtoBase<Post>, IFromEntity<Post, PostResponseDto>
{
    private PostResponseDto(Post entity)
        : base(entity)
    {
        ProfileId = entity.ProfileId;
        CategoryId = entity.CategoryId;
        Caption = entity.Caption;
        Status = entity.Status.ToString();
        UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt;
        Media = entity.Media
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(PostMediaResponseDto.FromEntity)
            .ToList();
        Targets = entity.Targets
            .OrderBy(x => x.TargetPlatform)
            .ThenBy(x => x.TargetAccountId)
            .Select(PostTargetResponseDto.FromEntity)
            .ToList();
    }

    public Guid ProfileId { get; init; }
    public Guid? CategoryId { get; init; }
    public string Caption { get; init; }
    public string Status { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public IReadOnlyList<PostMediaResponseDto> Media { get; init; }
    public IReadOnlyList<PostTargetResponseDto> Targets { get; init; }

    public static PostResponseDto FromEntity(Post entity)
        => new(entity);
}