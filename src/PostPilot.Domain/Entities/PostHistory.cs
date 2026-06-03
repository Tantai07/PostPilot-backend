using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostHistory : SoftDeleteEntity
{
    public Guid PostId { get; private set; }
    public PostTargetPlatform Platform { get; private set; }
    public string? ExternalPostId { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }
    public Post? Post { get; private set; }
}
