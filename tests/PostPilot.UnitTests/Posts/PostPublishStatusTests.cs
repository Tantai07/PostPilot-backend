using FluentAssertions;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Posts;

public sealed class PostPublishStatusTests
{
    [Fact]
    public void MarkPublishing_ChangesStatusToPublishing()
    {
        var post = new Post(Guid.NewGuid(), null, "Caption");

        post.MarkPublishing();

        post.Status.Should().Be(PostStatus.Publishing);
    }

    [Fact]
    public void MarkFailed_ChangesStatusToFailed()
    {
        var post = new Post(Guid.NewGuid(), null, "Caption");

        post.MarkFailed();

        post.Status.Should().Be(PostStatus.Failed);
    }
}