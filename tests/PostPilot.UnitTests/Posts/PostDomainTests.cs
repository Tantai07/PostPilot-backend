using FluentAssertions;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Posts;

public sealed class PostDomainTests
{
    [Fact]
    public void Constructor_CreatesDraftPost()
    {
        var profileId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var post = new Post(profileId, categoryId, "  Test caption  ");

        post.ProfileId.Should().Be(profileId);
        post.CategoryId.Should().Be(categoryId);
        post.Caption.Should().Be("Test caption");
        post.Status.Should().Be(PostStatus.Draft);
    }

    [Fact]
    public void MarkQueued_And_MarkPosted_UpdateStatus()
    {
        var post = new Post(Guid.NewGuid(), null, "Caption");

        post.MarkQueued();
        post.Status.Should().Be(PostStatus.Queued);

        post.MarkPosted();
        post.Status.Should().Be(PostStatus.Posted);
    }
}
