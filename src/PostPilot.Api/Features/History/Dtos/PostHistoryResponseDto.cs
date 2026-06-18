using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.History.Dtos;

public sealed class PostHistoryResponseDto : ResponseDtoBase<PostHistory>, IFromEntity<PostHistory, PostHistoryResponseDto>
{
    private PostHistoryResponseDto(PostHistory entity)
        : base(entity)
    {
        PostId = entity.PostId;
        ProfileId = entity.Post?.ProfileId ?? Guid.Empty;
        CategoryId = entity.Post?.CategoryId;
        Caption = entity.Post?.Caption ?? string.Empty;
        PostStatus = entity.Post?.Status.ToString() ?? string.Empty;
        Platform = entity.Platform.ToString();
        ExternalPostId = entity.ExternalPostId;
        Status = entity.Status;
        ErrorMessage = entity.ErrorMessage;
        CreatedAt = entity.CreatedAt;
    }

    public Guid PostId { get; init; }
    public Guid ProfileId { get; init; }
    public Guid? CategoryId { get; init; }
    public string Caption { get; init; }
    public string PostStatus { get; init; }
    public string Platform { get; init; }
    public string? ExternalPostId { get; init; }
    public string Status { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public static PostHistoryResponseDto FromEntity(PostHistory entity)
        => new(entity);
}