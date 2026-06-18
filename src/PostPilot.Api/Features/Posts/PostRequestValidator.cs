using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Posts;

public static class PostRequestValidator
{
    public static bool TryValidate(
        string caption,
        IEnumerable<PostMediaRequestDto>? media,
        IEnumerable<PostTargetRequestDto>? targets,
        out List<string> errors)
    {
        errors = [];

        if (string.IsNullOrWhiteSpace(caption))
        {
            errors.Add("Caption is required.");
        }

        ValidateMedia(media, errors);
        ValidateTargets(targets, errors);

        return errors.Count == 0;
    }

    public static IReadOnlyList<(StorageProvider StorageProvider, string Url, string? PublicUrl, int SortOrder)> MapMedia(
        IEnumerable<PostMediaRequestDto>? media)
    {
        if (media is null)
        {
            return [];
        }

        var result = new List<(StorageProvider StorageProvider, string Url, string? PublicUrl, int SortOrder)>();
        var index = 1;

        foreach (var item in media)
        {
            var provider = Enum.Parse<StorageProvider>(item.StorageProvider, ignoreCase: true);
            result.Add((provider, item.Url.Trim(), item.PublicUrl?.Trim(), item.SortOrder ?? index));
            index++;
        }

        return result;
    }

    public static IReadOnlyList<(PostTargetPlatform TargetPlatform, Guid TargetAccountId)> MapTargets(
        IEnumerable<PostTargetRequestDto>? targets)
    {
        if (targets is null)
        {
            return [];
        }

        return targets
            .Select(target => (
                Enum.Parse<PostTargetPlatform>(target.TargetPlatform, ignoreCase: true),
                target.TargetAccountId))
            .ToList();
    }

    private static void ValidateMedia(IEnumerable<PostMediaRequestDto>? media, List<string> errors)
    {
        if (media is null)
        {
            return;
        }

        foreach (var item in media)
        {
            if (string.IsNullOrWhiteSpace(item.Url))
            {
                errors.Add("Media URL is required.");
            }

            if (item.Url?.Length > 2048 || item.PublicUrl?.Length > 2048)
            {
                errors.Add("Media URL must be 2048 characters or fewer.");
            }

            if (!Enum.TryParse<StorageProvider>(item.StorageProvider, ignoreCase: true, out _))
            {
                errors.Add($"Unsupported storage provider: {item.StorageProvider}.");
            }
        }
    }

    private static void ValidateTargets(IEnumerable<PostTargetRequestDto>? targets, List<string> errors)
    {
        if (targets is null)
        {
            return;
        }

        foreach (var target in targets)
        {
            if (target.TargetAccountId == Guid.Empty)
            {
                errors.Add("Target account id is required.");
            }

            if (!Enum.TryParse<PostTargetPlatform>(target.TargetPlatform, ignoreCase: true, out _))
            {
                errors.Add($"Unsupported target platform: {target.TargetPlatform}.");
            }
        }
    }
}