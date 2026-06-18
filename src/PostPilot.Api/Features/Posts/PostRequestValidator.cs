using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Posts;

public static class PostRequestValidator
{
    public static bool TryValidateDraftInput(
        string caption,
        IEnumerable<Guid>? mediaIds,
        IEnumerable<string>? targetPlatforms,
        out List<string> errors)
    {
        errors = [];

        if (string.IsNullOrWhiteSpace(caption))
        {
            errors.Add("Caption is required.");
        }

        if (caption.Length > 4096)
        {
            errors.Add("Caption must be 4096 characters or fewer.");
        }

        if (mediaIds?.Any(x => x == Guid.Empty) == true)
        {
            errors.Add("Media id cannot be empty.");
        }

        foreach (var targetPlatform in targetPlatforms ?? [])
        {
            if (!TryParseTargetPlatform(targetPlatform, out _))
            {
                errors.Add($"Unsupported target platform: {targetPlatform}.");
            }
        }

        return errors.Count == 0;
    }

    public static bool TryParseTargetPlatform(string targetPlatform, out PostTargetPlatform platform)
    {
        switch (targetPlatform.Trim().ToLowerInvariant())
        {
            case "facebook page":
            case "facebookpage":
                platform = PostTargetPlatform.FacebookPage;
                return true;
            case "instagram feed":
            case "instagramfeed":
                platform = PostTargetPlatform.InstagramFeed;
                return true;
            case "instagram story":
            case "instagramstory":
                platform = PostTargetPlatform.InstagramStory;
                return true;
            default:
                platform = default;
                return false;
        }
    }

    public static IReadOnlyList<PostTargetPlatform> ParseTargetPlatforms(IEnumerable<string> targetPlatforms)
    {
        return targetPlatforms
            .Select(x => TryParseTargetPlatform(x, out var platform) ? platform : (PostTargetPlatform?)null)
            .Where(x => x is not null)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();
    }
}