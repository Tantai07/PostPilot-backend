using PostPilot.Domain.Common;

namespace PostPilot.Domain.Entities;

public sealed class MetaToken : SoftDeleteEntity
{
    private MetaToken()
    {
    }

    public MetaToken(
        Guid socialAccountId,
        string encryptedAccessToken,
        string? encryptedRefreshToken,
        DateTimeOffset expiresAt,
        string? scope)
    {
        SocialAccountId = socialAccountId;
        Update(encryptedAccessToken, encryptedRefreshToken, expiresAt, scope);
    }

    public Guid SocialAccountId { get; private set; }
    public string EncryptedAccessToken { get; private set; } = string.Empty;
    public string? EncryptedRefreshToken { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public string? Scope { get; private set; }
    public SocialAccount? SocialAccount { get; private set; }

    public void Update(string encryptedAccessToken, string? encryptedRefreshToken, DateTimeOffset expiresAt, string? scope)
    {
        EncryptedAccessToken = encryptedAccessToken;
        EncryptedRefreshToken = encryptedRefreshToken;
        ExpiresAt = expiresAt;
        Scope = string.IsNullOrWhiteSpace(scope) ? null : scope.Trim();
    }
}
