using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Publishing;

public sealed class MockPostPublisher : IPostPublisher
{
    public Task<PublishResult> PublishAsync(
        Guid postId,
        PostTargetPlatform platform,
        CancellationToken cancellationToken)
    {
        var externalPostId = $"mock-{platform.ToString().ToLowerInvariant()}-{postId:N}";
        var result = new PublishResult(platform, true, externalPostId, null);
        return Task.FromResult(result);
    }
}
