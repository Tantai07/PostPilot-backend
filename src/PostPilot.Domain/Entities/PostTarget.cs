using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostTarget : SoftDeleteEntity
{
    private PostTarget()
    {
    }

    public PostTarget(Guid postId, PostTargetPlatform targetPlatform, Guid targetAccountId)
    {
        PostId = postId;
        TargetPlatform = targetPlatform;
        TargetAccountId = targetAccountId;
    }

    public Guid PostId { get; private set; }
    public PostTargetPlatform TargetPlatform { get; private set; }
    public Guid TargetAccountId { get; private set; }
    public Post? Post { get; private set; }
}