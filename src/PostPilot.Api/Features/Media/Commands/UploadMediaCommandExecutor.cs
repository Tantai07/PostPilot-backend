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
        "image/gif"
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

        if (file.Length <= 0 || file.Length > MaxFileSizeBytes)
        {
            throw new InvalidOperationException("Image must be between 1 byte and 10 MB.");
        }

        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Only JPG, PNG, WebP, and GIF images are supported.");
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