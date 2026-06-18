using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PostPilot.Api.Features.Media;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Media.Storage;

public sealed class LocalMediaStorageService : IMediaStorageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalMediaStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<StoredMediaFile> SaveAsync(
        Guid profileId,
        IFormFile file,
        Uri publicBaseUri,
        CancellationToken cancellationToken)
    {
        var extension = MediaUploadValidator.GetSafeExtension(file.ContentType);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var relativeUrl = $"/uploads/profiles/{profileId:D}/{storedFileName}";
        var webRootPath = _environment.WebRootPath;

        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var targetDirectory = Path.Combine(webRootPath, "uploads", "profiles", profileId.ToString("D"));
        Directory.CreateDirectory(targetDirectory);

        var physicalPath = Path.Combine(targetDirectory, storedFileName);
        await using (var stream = File.Create(physicalPath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var publicUrl = new Uri(publicBaseUri, relativeUrl.TrimStart('/')).ToString();

        return new StoredMediaFile(
            StorageProvider.Local,
            relativeUrl,
            publicUrl,
            Path.GetFileName(file.FileName),
            file.ContentType,
            file.Length);
    }
}