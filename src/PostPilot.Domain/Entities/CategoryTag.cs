using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class CategoryTag : SoftDeleteEntity
{
    private CategoryTag()
    {
    }

    public CategoryTag(Guid categoryId, string tagText, int sortOrder)
    {
        CategoryId = categoryId;
        Update(tagText, sortOrder);
    }

    public Guid CategoryId { get; private set; }
    public string TagText { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public Category? Category { get; private set; }

    public void Update(string tagText, int sortOrder)
    {
        TagText = tagText.Trim();
        SortOrder = sortOrder;
    }
}