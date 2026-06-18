using FluentAssertions;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Queue;

public sealed class PostQueueItemDomainTests
{
    [Fact]
    public void Constructor_CreatesPendingQueueItem()
    {
        var profileId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var scheduledAt = DateTimeOffset.UtcNow.AddHours(1);

        var item = new PostQueueItem(profileId, postId, 3, scheduledAt);

        item.ProfileId.Should().Be(profileId);
        item.PostId.Should().Be(postId);
        item.SortOrder.Should().Be(3);
        item.ScheduledAt.Should().Be(scheduledAt);
        item.Status.Should().Be(QueueItemStatus.Pending);
    }

    [Fact]
    public void MoveTo_UpdatesSortOrder()
    {
        var item = new PostQueueItem(Guid.NewGuid(), Guid.NewGuid(), 3);

        item.MoveTo(1);

        item.SortOrder.Should().Be(1);
    }

    [Fact]
    public void MarkPosted_UpdatesStatus()
    {
        var item = new PostQueueItem(Guid.NewGuid(), Guid.NewGuid(), 1);

        item.MarkPosted();

        item.Status.Should().Be(QueueItemStatus.Posted);
    }
}