using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Media.Storage;

public sealed record StoredMediaFile(
    StorageProvider StorageProvider,
    string Url,
    string PublicUrl,
    string OriginalFileName,
    string ContentType,
    long SizeBytes);