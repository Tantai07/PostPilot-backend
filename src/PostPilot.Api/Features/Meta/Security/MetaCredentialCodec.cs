using Microsoft.AspNetCore.DataProtection;

namespace PostPilot.Api.Features.Meta.Security;

public sealed class MetaCredentialCodec
{
    private readonly IDataProtector _protector;

    public MetaCredentialCodec(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("PostPilot.MetaCredentials.v1");
    }

    public string Encode(string value) => _protector.Protect(value);

    public string Decode(string value) => _protector.Unprotect(value);
}
