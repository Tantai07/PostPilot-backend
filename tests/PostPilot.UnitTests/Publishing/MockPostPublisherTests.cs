using FluentAssertions;
using PostPilot.Api.Features.Publishing;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Publishing;

public sealed class MockPostPublisherTests
{
    [Fact]
    public async Task PublishAsync_ReturnsSuccessfulMockResult()
    {
        var publisher = new MockPostPublisher();
        var postId = Guid.NewGuid();

        var result = await publisher.PublishAsync(postId, PostTargetPlatform.FacebookPage, CancellationToken.None);

        result.Platform.Should().Be(PostTargetPlatform.FacebookPage);
        result.IsSuccess.Should().BeTrue();
        result.ExternalPostId.Should().Be($"mock-facebookpage-{postId:N}");
        result.ErrorMessage.Should().BeNull();
    }
}