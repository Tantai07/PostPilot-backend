using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class Category : SoftDeleteEntity
{
    private readonly List<CategoryTag> _tags = [];

    public Guid ProfileId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Profile? Profile { get; private set; }
    public IReadOnlyCollection<CategoryTag> Tags => _tags;
}
