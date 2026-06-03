using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class Profile : SoftDeleteEntity
{
    private readonly List<SocialAccount> _socialAccounts = [];
    private readonly List<Category> _categories = [];

    private Profile()
    {
    }

    public Profile(Guid ownerUserId, string name, string? websiteName, string? defaultTargets)
    {
        OwnerUserId = ownerUserId;
        Rename(name, websiteName, defaultTargets);
    }

    public Guid OwnerUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? WebsiteName { get; private set; }
    public string? DefaultTargets { get; private set; }
    public IReadOnlyCollection<SocialAccount> SocialAccounts => _socialAccounts;
    public IReadOnlyCollection<Category> Categories => _categories;

    public void Rename(string name, string? websiteName, string? defaultTargets)
    {
        Name = name.Trim();
        WebsiteName = string.IsNullOrWhiteSpace(websiteName) ? null : websiteName.Trim();
        DefaultTargets = string.IsNullOrWhiteSpace(defaultTargets) ? null : defaultTargets.Trim();
    }
}
