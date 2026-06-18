using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Queue.Dtos;

public sealed class QueueItemResponseDto : ResponseDtoBase<PostQueueItem>, IFromEntity<PostQueueItem, QueueItemResponseDto>
{
    private QueueItemResponseDto(PostQueueItem entity)
        : base(entity)
    {
        ProfileId = entity.ProfileId;
        PostId = entity.PostId;
        SortOrder = entity.SortOrder;
        ScheduledAt = entity.ScheduledAt;
        Status = entity.Status.ToString();
        Caption = entity.Post?.Caption ?? string.Empty;
        CategoryId = entity.Post?.CategoryId;
        TargetPlatforms = entity.Post?.Targets
            .Where(x => x.DeletedAt == null)
            .Select(x => x.TargetPlatform.ToString())
            .ToList() ?? [];
    }

    public Guid ProfileId { get; init; }
    public Guid PostId { get; init; }
    public int SortOrder { get; init; }
    public DateTimeOffset? ScheduledAt { get; init; }
    public string Status { get; init; }
    public string Caption { get; init; }
    public Guid? CategoryId { get; init; }
    public IReadOnlyCollection<string> TargetPlatforms { get; init; }

    public static QueueItemResponseDto FromEntity(PostQueueItem entity)
        => new(entity);
}