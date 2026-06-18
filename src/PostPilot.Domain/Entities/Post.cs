using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class Post : SoftDeleteEntity
{
    private readonly List<PostMedia> _media = [];
    private readonly List<PostTarget> _targets = [];

    private Post()
    {
    }

    public Post(Guid profileId, Guid? categoryId, string caption)
    {
        ProfileId = profileId;
        CategoryId = categoryId;
        Caption = caption.Trim();
        Status = PostStatus.Draft;
    }

    public Guid ProfileId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string Caption { get; private set; } = string.Empty;
    public PostStatus Status { get; private set; } = PostStatus.Draft;
    public Profile? Profile { get; private set; }
    public Category? Category { get; private set; }
    public IReadOnlyCollection<PostMedia> Media => _media;
    public IReadOnlyCollection<PostTarget> Targets => _targets;

    public void AddMedia(StorageProvider storageProvider, string url, string? publicUrl, int sortOrder)
    {
        _media.Add(new PostMedia(Id, storageProvider, url, publicUrl, sortOrder));
    }

    public void AddTarget(PostTargetPlatform targetPlatform, Guid targetAccountId)
    {
        _targets.Add(new PostTarget(Id, targetPlatform, targetAccountId));
    }

    public void MarkQueued()
    {
        Status = PostStatus.Queued;
    }

    public void MarkPosted()
    {
        Status = PostStatus.Posted;
    }
}