using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostQueueItem : SoftDeleteEntity
{
    public Guid ProfileId { get; private set; }
    public Guid PostId { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset? ScheduledAt { get; private set; }
    public QueueItemStatus Status { get; private set; } = QueueItemStatus.Pending;
    public Profile? Profile { get; private set; }
    public Post? Post { get; private set; }
}
