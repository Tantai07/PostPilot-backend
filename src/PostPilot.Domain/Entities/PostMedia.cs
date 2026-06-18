using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class PostMedia : SoftDeleteEntity
{
    private PostMedia()
    {
    }

    public PostMedia(Guid postId, StorageProvider storageProvider, string url, string? publicUrl, int sortOrder)
    {
        PostId = postId;
        StorageProvider = storageProvider;
        Url = url.Trim();
        PublicUrl = string.IsNullOrWhiteSpace(publicUrl) ? null : publicUrl.Trim();
        SortOrder = sortOrder;
    }

    public Guid PostId { get; private set; }
    public StorageProvider StorageProvider { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string? PublicUrl { get; private set; }
    public int SortOrder { get; private set; }
    public Post? Post { get; private set; }
}