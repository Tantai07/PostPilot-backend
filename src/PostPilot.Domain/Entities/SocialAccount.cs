using PostPilot.Domain.Common;
using PostPilot.Domain.Enums;

namespace PostPilot.Domain.Entities;

public sealed class SocialAccount : SoftDeleteEntity
{
    private SocialAccount()
    {
    }

    public SocialAccount(
        Guid profileId,
        SocialPlatform platform,
        string pageId,
        string? igUserId,
        string displayName)
    {
        ProfileId = profileId;
        Platform = platform;
        Update(pageId, igUserId, displayName);
    }

    public Guid ProfileId { get; private set; }
    public SocialPlatform Platform { get; private set; }
    public string PageId { get; private set; } = string.Empty;
    public string? IgUserId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public Profile? Profile { get; private set; }
    public MetaToken? MetaToken { get; private set; }

    public void Update(string pageId, string? igUserId, string displayName)
    {
        PageId = pageId.Trim();
        IgUserId = string.IsNullOrWhiteSpace(igUserId) ? null : igUserId.Trim();
        DisplayName = displayName.Trim();
    }
}