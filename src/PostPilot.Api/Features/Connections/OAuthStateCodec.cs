using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Connections;

public sealed record OAuthStatePayload(
    Guid UserId,
    Guid ProfileId,
    SocialPlatform Platform,
    string CodeVerifier,
    DateTimeOffset ExpiresAt);

public sealed class OAuthStateCodec
{
    private readonly IDataProtector _protector;

    public OAuthStateCodec(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("PostPilot.OAuthState.v1");
    }

    public string Encode(OAuthStatePayload payload)
        => _protector.Protect(JsonSerializer.Serialize(payload));

    public OAuthStatePayload Decode(string value)
    {
        var payload = JsonSerializer.Deserialize<OAuthStatePayload>(_protector.Unprotect(value))
            ?? throw new InvalidOperationException("Invalid OAuth state.");

        if (payload.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("OAuth state has expired.");
        }

        return payload;
    }
}
