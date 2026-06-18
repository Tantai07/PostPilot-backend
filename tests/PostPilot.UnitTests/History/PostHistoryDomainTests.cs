using FluentAssertions;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.History;

public sealed class PostHistoryDomainTests
{
    [Fact]
    public void Constructor_TrimsStatusExternalIdAndError()
    {
        var postId = Guid.NewGuid();

        var history = new PostHistory(
            postId,
            PostTargetPlatform.FacebookPage,
            "  Posted  ",
            "  mock-id  ",
            "  warning  ");

        history.PostId.Should().Be(postId);
        history.Platform.Should().Be(PostTargetPlatform.FacebookPage);
        history.Status.Should().Be("Posted");
        history.ExternalPostId.Should().Be("mock-id");
        history.ErrorMessage.Should().Be("warning");
    }

    [Fact]
    public void Constructor_StoresBlankExternalIdAndErrorAsNull()
    {
        var history = new PostHistory(
            Guid.NewGuid(),
            PostTargetPlatform.InstagramFeed,
            "Posted",
            " ",
            " ");

        history.ExternalPostId.Should().BeNull();
        history.ErrorMessage.Should().BeNull();
    }
}