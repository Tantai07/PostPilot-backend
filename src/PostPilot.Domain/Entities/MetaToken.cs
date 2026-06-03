using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class MetaToken : SoftDeleteEntity
{
    public Guid SocialAccountId { get; private set; }
    public string EncryptedAccessToken { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public SocialAccount? SocialAccount { get; private set; }
}
