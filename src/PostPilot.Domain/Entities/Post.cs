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
        UpdateCaptionAndCategory(caption, categoryId);
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

    public void UpdateCaptionAndCategory(string caption, Guid? categoryId)
    {
        Caption = caption.Trim();
        CategoryId = categoryId;
    }

    public void AddMedia(StorageProvider storageProvider, string url, string? publicUrl, int sortOrder, Guid? createdBy)
    {
        _media.Add(new PostMedia(Id, storageProvider, url, publicUrl, sortOrder)
        {
            CreatedBy = createdBy
        });
    }

    public void AddTarget(PostTargetPlatform targetPlatform, Guid targetAccountId, Guid? createdBy)
    {
        _targets.Add(new PostTarget(Id, targetPlatform, targetAccountId)
        {
            CreatedBy = createdBy
        });
    }
}