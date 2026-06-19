using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Media.Storage;

public sealed record MediaStorageResult(
    StorageProvider Provider,
    string Url,
    string PublicUrl,
    string FileName,
    string MimeType,
    long SizeBytes);
