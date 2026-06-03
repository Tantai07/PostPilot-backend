using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostTarget : SoftDeleteEntity
{
    public Guid PostId { get; private set; }
    public PostTargetPlatform TargetPlatform { get; private set; }
    public Guid TargetAccountId { get; private set; }
    public Post? Post { get; private set; }
}
