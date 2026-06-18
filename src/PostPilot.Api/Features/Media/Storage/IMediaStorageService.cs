namespace PostPilot.Api.Features.Media.Storage;

public interface IMediaStorageService
{
    Task<StoredMediaFile> SaveAsync(Guid profileId, IFormFile file, Uri publicBaseUri, CancellationToken cancellationToken);
}