using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Media.Dtos;
using PostPilot.Api.Features.Media.Storage;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Media.Commands;

public sealed class UploadMediaCommandExecutor
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
        "video/mp4",
        "video/webm",
        "video/quicktime"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;
    private readonly AppDbContext _dbContext;
    private readonly IMediaStorageService _storageService;

    public UploadMediaCommandExecutor(AppDbContext dbContext, IMediaStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<MediaUploadResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        IFormFile file,
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var maxSize = file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ? 100 * 1024 * 1024 : MaxFileSizeBytes;
        if (file.Length <= 0 || file.Length > maxSize)
        {
            throw new InvalidOperationException("Images must be at most 10 MB; videos at most 100 MB. Empty files are not supported.");
        }

        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Only JPG, PNG, WebP, GIF, MP4, WebM, and MOV files are supported.");
        }

        var storageResult = await _storageService.UploadAsync(profileId, file, request, cancellationToken);
        var media = new MediaAsset(
            profileId,
            storageResult.Provider,
            storageResult.Url,
            storageResult.PublicUrl,
            storageResult.FileName,
            storageResult.MimeType,
            storageResult.SizeBytes)
        {
            CreatedBy = ownerUserId
        };

        _dbContext.MediaAssets.Add(media);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return media.ToDto();
    }
}