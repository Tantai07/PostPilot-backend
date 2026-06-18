namespace PostPilot.Api.Features.Media.Dtos;

public sealed class MediaUploadResponseDto
{
    public Guid ProfileId { get; init; }
    public string StorageProvider { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string PublicUrl { get; init; } = string.Empty;
    public string OriginalFileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public int SortOrder { get; init; }
    public DateTimeOffset UploadedAt { get; init; }
}