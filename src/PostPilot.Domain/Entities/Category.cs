using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class Category : SoftDeleteEntity
{
    private readonly List<CategoryTag> _tags = [];

    private Category()
    {
    }

    public Category(
        Guid profileId,
        string name,
        string? color,
        string? description,
        string? captionTemplate,
        IEnumerable<string>? tags)
    {
        ProfileId = profileId;
        Rename(name, color, description, captionTemplate);
        ReplaceTags(tags);
    }

    public Guid ProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CaptionTemplate { get; private set; }
    public Profile? Profile { get; private set; }
    public IReadOnlyCollection<CategoryTag> Tags => _tags;

    public void Rename(string name, string? color, string? description, string? captionTemplate)
    {
        Name = name.Trim();
        Color = string.IsNullOrWhiteSpace(color) ? "#F1F5F2" : color.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CaptionTemplate = string.IsNullOrWhiteSpace(captionTemplate) ? null : captionTemplate.Trim();
    }

    public void ReplaceTags(IEnumerable<string>? tags)
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var existingTag in _tags.Where(x => !x.IsDeleted))
        {
            existingTag.SoftDelete(null, now);
        }

        var nextTags = (tags ?? [])
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(30)
            .Select((tagText, index) => new CategoryTag(Id, tagText, index + 1));

        _tags.AddRange(nextTags);
    }

    public void SoftDeleteWithTags(Guid? deletedBy, DateTimeOffset deletedAt)
    {
        SoftDelete(deletedBy, deletedAt);

        foreach (var tag in _tags.Where(x => !x.IsDeleted))
        {
            tag.SoftDelete(deletedBy, deletedAt);
        }
    }
}