using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class MetaToken : SoftDeleteEntity
{
    private MetaToken()
    {
    }

    public MetaToken(Guid socialAccountId, string encryptedAccessToken, DateTimeOffset expiresAt)
    {
        SocialAccountId = socialAccountId;
        Update(encryptedAccessToken, expiresAt);
    }

    public Guid SocialAccountId { get; private set; }
    public string EncryptedAccessToken { get; private set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; private set; }
    public SocialAccount? SocialAccount { get; private set; }

    public void Update(string encryptedAccessToken, DateTimeOffset expiresAt)
    {
        EncryptedAccessToken = encryptedAccessToken;
        ExpiresAt = expiresAt;
    }
}