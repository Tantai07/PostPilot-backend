using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Publishing;

public interface IPostPublisher
{
    Task<PublishResult> PublishAsync(Guid postId, PostTargetPlatform platform, CancellationToken cancellationToken);
}

public sealed record PublishResult(
    PostTargetPlatform Platform,
    bool IsSuccess,
    string? ExternalPostId,
    string? ErrorMessage);
