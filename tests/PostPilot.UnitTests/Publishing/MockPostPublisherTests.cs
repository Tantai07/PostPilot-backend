using FluentAssertions;
using PostPilot.Api.Features.Publishing;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Publishing;

public sealed class MockPostPublisherTests
{
    [Fact]
    public async Task PublishAsync_ReturnsSuccessfulMockExternalId()
    {
        var postId = Guid.NewGuid();
        var publisher = new MockPostPublisher();

        var result = await publisher.PublishAsync(postId, PostTargetPlatform.FacebookPage, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Platform.Should().Be(PostTargetPlatform.FacebookPage);
        result.ExternalPostId.Should().StartWith("mock-facebookpage-");
        result.ExternalPostId.Should().EndWith(postId.ToString("N"));
        result.ErrorMessage.Should().BeNull();
    }
}
