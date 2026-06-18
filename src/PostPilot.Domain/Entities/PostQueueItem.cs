using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostQueueItem : SoftDeleteEntity
{
    private PostQueueItem()
    {
    }

    public PostQueueItem(Guid profileId, Guid postId, int sortOrder, DateTimeOffset? scheduledAt = null)
    {
        ProfileId = profileId;
        PostId = postId;
        SortOrder = sortOrder;
        ScheduledAt = scheduledAt;
        Status = QueueItemStatus.Pending;
    }

    public Guid ProfileId { get; private set; }
    public Guid PostId { get; private set; }
    public int SortOrder { get; private set; }
    public DateTimeOffset? ScheduledAt { get; private set; }
    public QueueItemStatus Status { get; private set; } = QueueItemStatus.Pending;
    public Profile? Profile { get; private set; }
    public Post? Post { get; private set; }

    public void MoveTo(int sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void MarkPosted()
    {
        Status = QueueItemStatus.Posted;
    }
}