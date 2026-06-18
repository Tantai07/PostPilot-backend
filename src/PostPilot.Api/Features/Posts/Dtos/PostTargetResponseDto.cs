using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Posts.Dtos;

public sealed class PostTargetResponseDto : ResponseDtoBase<PostTarget>, IFromEntity<PostTarget, PostTargetResponseDto>
{
    private PostTargetResponseDto(PostTarget entity)
        : base(entity)
    {
        TargetPlatform = entity.TargetPlatform.ToString();
        TargetAccountId = entity.TargetAccountId;
    }

    public string TargetPlatform { get; init; }
    public Guid TargetAccountId { get; init; }

    public static PostTargetResponseDto FromEntity(PostTarget entity)
        => new(entity);
}