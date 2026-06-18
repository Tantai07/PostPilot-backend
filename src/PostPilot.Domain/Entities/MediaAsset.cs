using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class MediaAsset : SoftDeleteEntity
{
    private MediaAsset()
    {
    }

    public MediaAsset(
        Guid profileId,
        StorageProvider storageProvider,
        string url,
        string publicUrl,
        string fileName,
        string mimeType,
        long sizeBytes)
    {
        ProfileId = profileId;
        StorageProvider = storageProvider;
        Url = url.Trim();
        PublicUrl = publicUrl.Trim();
        FileName = fileName.Trim();
        MimeType = mimeType.Trim();
        SizeBytes = sizeBytes;
    }

    public Guid ProfileId { get; private set; }
    public StorageProvider StorageProvider { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string PublicUrl { get; private set; } = string.Empty;
    public string FileName { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public Profile? Profile { get; private set; }
}