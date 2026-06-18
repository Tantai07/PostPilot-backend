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
        Media = entity.Media
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => new PostMediaResponseDto
            {
                Id = x.Id,
                Url = x.Url,
                PublicUrl = x.PublicUrl,
                StorageProvider = x.StorageProvider.ToString(),
                SortOrder = x.SortOrder
            })
            .ToList();
        TargetPlatforms = entity.Targets
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.TargetPlatform)
            .Select(x => x.TargetPlatform.ToString())
            .ToList();
        UpdatedAt = entity.UpdatedAt ?? entity.CreatedAt;
    }

    public Guid ProfileId { get; init; }
    public Guid? CategoryId { get; init; }
    public string Caption { get; init; }
    public string Status { get; init; }
    public IReadOnlyCollection<PostMediaResponseDto> Media { get; init; }
    public IReadOnlyCollection<string> TargetPlatforms { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public static PostResponseDto FromEntity(Post entity)
        => new(entity);
}

public sealed class PostMediaResponseDto
{
    public Guid Id { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? PublicUrl { get; init; }
    public string StorageProvider { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}