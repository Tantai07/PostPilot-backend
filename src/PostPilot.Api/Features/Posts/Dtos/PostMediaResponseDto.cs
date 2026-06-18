using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostMediaResponseDto : ResponseDtoBase<PostMedia>, IFromEntity<PostMedia, PostMediaResponseDto>
{
    private PostMediaResponseDto(PostMedia entity)
        : base(entity)
    {
        StorageProvider = entity.StorageProvider.ToString();
        Url = entity.Url;
        PublicUrl = entity.PublicUrl;
        SortOrder = entity.SortOrder;
    }

    public string StorageProvider { get; init; }
    public string Url { get; init; }
    public string? PublicUrl { get; init; }
    public int SortOrder { get; init; }

    public static PostMediaResponseDto FromEntity(PostMedia entity)
        => new(entity);
}