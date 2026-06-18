using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Media.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
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
    private readonly IWebHostEnvironment _environment;

    public UploadMediaCommandExecutor(AppDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
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

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = file.ContentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".img"
            };
        }

        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var relativeDirectory = Path.Combine("uploads", profileId.ToString("N"));
        var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var directoryPath = Path.Combine(webRootPath, relativeDirectory);
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, safeFileName);
        await using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var relativeUrl = $"/uploads/{profileId:N}/{safeFileName}";
        var publicUrl = $"{request.Scheme}://{request.Host}{relativeUrl}";

        var media = new MediaAsset(
            profileId,
            StorageProvider.Local,
            relativeUrl,
            publicUrl,
            file.FileName,
            file.ContentType,
            file.Length)
        {
            CreatedBy = ownerUserId
        };

        _dbContext.MediaAssets.Add(media);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return media.ToDto();
    }
}