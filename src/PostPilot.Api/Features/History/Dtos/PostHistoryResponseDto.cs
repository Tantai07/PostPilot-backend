using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.History.Dtos;

public sealed class PostHistoryResponseDto : ResponseDtoBase<PostHistory>, IFromEntity<PostHistory, PostHistoryResponseDto>
{
    private PostHistoryResponseDto(PostHistory entity)
        : base(entity)
    {
        PostId = entity.PostId;
        Caption = entity.Post?.Caption ?? string.Empty;
        CategoryId = entity.Post?.CategoryId;
        Platform = entity.Platform.ToString();
        Status = entity.Status;
        ExternalPostId = entity.ExternalPostId;
        ErrorMessage = entity.ErrorMessage;
        PublishedAt = entity.CreatedAt;
    }

    public Guid PostId { get; init; }
    public string Caption { get; init; }
    public Guid? CategoryId { get; init; }
    public string Platform { get; init; }
    public string Status { get; init; }
    public string? ExternalPostId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTimeOffset PublishedAt { get; init; }

    public static PostHistoryResponseDto FromEntity(PostHistory entity)
        => new(entity);
}