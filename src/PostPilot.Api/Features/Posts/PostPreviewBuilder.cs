namespace PostPilot.Api.Features.Posts;

public static class PostPreviewBuilder
{
    public static string BuildCaption(string caption, IEnumerable<string> categoryTags)
    {
        var cleanCaption = caption.Trim();
        var tags = categoryTags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (tags.Count == 0)
        {
            return cleanCaption;
        }

        var tagLine = string.Join(" ", tags);
        if (string.IsNullOrWhiteSpace(cleanCaption))
        {
            return tagLine;
        }

        return $"{cleanCaption}{Environment.NewLine}{Environment.NewLine}{tagLine}";
    }
}