using PostPilot.Api.Shared;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Media.Dtos;

public sealed class MediaUploadResponseDto : ResponseDtoBase<MediaAsset>, IFromEntity<MediaAsset, MediaUploadResponseDto>
{
    private MediaUploadResponseDto(MediaAsset entity)
        : base(entity)
    {
        ProfileId = entity.ProfileId;
        Url = entity.Url;
        PublicUrl = entity.PublicUrl;
        FileName = entity.FileName;
        MimeType = entity.MimeType;
        SizeBytes = entity.SizeBytes;
        StorageProvider = entity.StorageProvider.ToString();
        UploadedAt = entity.CreatedAt;
    }

    public Guid ProfileId { get; init; }
    public string Url { get; init; }
    public string PublicUrl { get; init; }
    public string FileName { get; init; }
    public string MimeType { get; init; }
    public long SizeBytes { get; init; }
    public string StorageProvider { get; init; }
    public DateTimeOffset UploadedAt { get; init; }

    public static MediaUploadResponseDto FromEntity(MediaAsset entity)
        => new(entity);
}