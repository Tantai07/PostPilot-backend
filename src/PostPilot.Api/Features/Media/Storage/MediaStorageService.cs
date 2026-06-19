using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Media.Storage;

public sealed class MediaStorageService : IMediaStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly HttpClient _httpClient;
    private readonly MediaStorageOptions _options;

    public MediaStorageService(
        IWebHostEnvironment environment,
        HttpClient httpClient,
        IOptions<MediaStorageOptions> options)
    {
        _environment = environment;
        _httpClient = httpClient;
        _options = options.Value;
    }

    public Task<MediaStorageResult> UploadAsync(
        Guid profileId,
        IFormFile file,
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        return IsCloudinaryEnabled()
            ? UploadToCloudinaryAsync(profileId, file, cancellationToken)
            : UploadToLocalAsync(profileId, file, request, cancellationToken);
    }

    private bool IsCloudinaryEnabled()
    {
        return _options.Provider.Equals("Cloudinary", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(_options.CloudinaryCloudName)
            && !string.IsNullOrWhiteSpace(_options.CloudinaryApiKey)
            && !string.IsNullOrWhiteSpace(_options.CloudinaryApiSecret);
    }

    private async Task<MediaStorageResult> UploadToLocalAsync(
        Guid profileId,
        IFormFile file,
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        var extension = GetFileExtension(file);
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

        return new MediaStorageResult(
            StorageProvider.Local,
            relativeUrl,
            publicUrl,
            file.FileName,
            file.ContentType,
            file.Length);
    }

    private async Task<MediaStorageResult> UploadToCloudinaryAsync(
        Guid profileId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var publicId = $"{profileId:N}/{Guid.NewGuid():N}";
        var folder = _options.CloudinaryFolder.Trim('/');
        var signature = CreateCloudinarySignature(new Dictionary<string, string>
        {
            ["folder"] = folder,
            ["public_id"] = publicId,
            ["timestamp"] = timestamp
        });

        using var form = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        using var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        form.Add(fileContent, "file", file.FileName);
        form.Add(new StringContent(_options.CloudinaryApiKey), "api_key");
        form.Add(new StringContent(timestamp), "timestamp");
        form.Add(new StringContent(folder), "folder");
        form.Add(new StringContent(publicId), "public_id");
        form.Add(new StringContent(signature), "signature");

        var url = $"https://api.cloudinary.com/v1_1/{_options.CloudinaryCloudName}/image/upload";
        using var response = await _httpClient.PostAsync(url, form, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Cloudinary upload failed.");
        }

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;
        var secureUrl = root.GetProperty("secure_url").GetString() ?? string.Empty;
        var assetPublicId = root.GetProperty("public_id").GetString() ?? publicId;
        var bytes = root.TryGetProperty("bytes", out var bytesElement) ? bytesElement.GetInt64() : file.Length;

        return new MediaStorageResult(
            StorageProvider.Cloudinary,
            assetPublicId,
            secureUrl,
            file.FileName,
            file.ContentType,
            bytes);
    }

    private string CreateCloudinarySignature(IReadOnlyDictionary<string, string> parameters)
    {
        var rawSignature = string.Join("&", parameters
            .OrderBy(x => x.Key, StringComparer.Ordinal)
            .Select(x => $"{x.Key}={x.Value}")) + _options.CloudinaryApiSecret;

        var bytes = SHA1.HashData(Encoding.UTF8.GetBytes(rawSignature));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string GetFileExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(extension))
        {
            return extension;
        }

        return file.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => ".img"
        };
    }
}
