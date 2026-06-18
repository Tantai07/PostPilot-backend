using Microsoft.AspNetCore.Http;

namespace PostPilot.Api.Features.Media;

public static class MediaUploadValidator
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public static bool TryValidate(IFormFile? file, out string? errorMessage)
    {
        if (file is null)
        {
            errorMessage = "Image file is required.";
            return false;
        }

        if (file.Length <= 0)
        {
            errorMessage = "Image file is empty.";
            return false;
        }

        if (file.Length > MaxFileSizeBytes)
        {
            errorMessage = "Image file must be 10 MB or smaller.";
            return false;
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            errorMessage = "Only JPEG, PNG, and WebP images are supported.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public static string GetSafeExtension(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new InvalidOperationException("Unsupported media content type.")
        };
    }
}