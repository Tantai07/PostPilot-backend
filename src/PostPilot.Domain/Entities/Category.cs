using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class Category : SoftDeleteEntity
{
    private readonly List<CategoryTag> _tags = [];

    private Category()
    {
    }

    public Category(Guid profileId, string name, string color, string? description)
    {
        ProfileId = profileId;
        UpdateDetails(name, color, description);
    }

    public Guid ProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Profile? Profile { get; private set; }
    public IReadOnlyCollection<CategoryTag> Tags => _tags;

    public void UpdateDetails(string name, string color, string? description)
    {
        Name = name.Trim();
        Color = color.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void ReplaceTags(
        IEnumerable<(string TagText, int SortOrder)> tags,
        Guid? changedBy,
        DateTimeOffset changedAt)
    {
        foreach (var existingTag in _tags.Where(x => !x.IsDeleted))
        {
            existingTag.SoftDelete(changedBy, changedAt);
        }

        foreach (var tag in tags.Where(x => !string.IsNullOrWhiteSpace(x.TagText)))
        {
            _tags.Add(new CategoryTag(Id, tag.TagText, tag.SortOrder)
            {
                CreatedBy = changedBy
            });
        }
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