using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostHistory : SoftDeleteEntity
{
    private PostHistory()
    {
    }

    public PostHistory(
        Guid postId,
        PostTargetPlatform platform,
        string status,
        string? externalPostId,
        string? errorMessage = null)
    {
        PostId = postId;
        Platform = platform;
        Status = status.Trim();
        ExternalPostId = string.IsNullOrWhiteSpace(externalPostId) ? null : externalPostId.Trim();
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? null : errorMessage.Trim();
    }

    public Guid PostId { get; private set; }
    public PostTargetPlatform Platform { get; private set; }
    public string? ExternalPostId { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }
    public Post? Post { get; private set; }
}