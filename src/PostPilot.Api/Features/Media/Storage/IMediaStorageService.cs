namespace PostPilot.Api.Features.Media.Storage;

public interface IMediaStorageService
{
    Task<MediaStorageResult> UploadAsync(
        Guid profileId,
        IFormFile file,
        HttpRequest request,
        CancellationToken cancellationToken);
}
