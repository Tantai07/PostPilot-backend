using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Media.Dtos;
using PostPilot.Api.Features.Media.Storage;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Media.Commands;

public sealed class UploadMediaCommandExecutor
{
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
        UploadMediaRequestDto request,
        Uri publicBaseUri,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId, cancellationToken);

        if (!profileExists || request.File is null)
        {
            return null;
        }

        var storedFile = await _storageService.SaveAsync(profileId, request.File, publicBaseUri, cancellationToken);

        return new MediaUploadResponseDto
        {
            ProfileId = profileId,
            StorageProvider = storedFile.StorageProvider.ToString(),
            Url = storedFile.Url,
            PublicUrl = storedFile.PublicUrl,
            OriginalFileName = storedFile.OriginalFileName,
            ContentType = storedFile.ContentType,
            SizeBytes = storedFile.SizeBytes,
            SortOrder = request.SortOrder,
            UploadedAt = DateTimeOffset.UtcNow
        };
    }
}