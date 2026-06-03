using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class Post : SoftDeleteEntity
{
    private readonly List<PostMedia> _media = [];
    private readonly List<PostTarget> _targets = [];

    public Guid ProfileId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Caption { get; private set; } = string.Empty;
    public PostStatus Status { get; private set; } = PostStatus.Draft;
    public Profile? Profile { get; private set; }
    public Category? Category { get; private set; }
    public IReadOnlyCollection<PostMedia> Media => _media;
    public IReadOnlyCollection<PostTarget> Targets => _targets;
}
