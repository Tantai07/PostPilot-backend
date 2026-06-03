using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class CategoryTag : SoftDeleteEntity
{
    public Guid CategoryId { get; private set; }
    public string TagText { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public Category? Category { get; private set; }
}
