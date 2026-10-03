using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostPilot.Api.Features.Meta.Security;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Connections;

public sealed class OAuthConnectionService
{
    private readonly AppDbContext _dbContext;
    private readonly MetaCredentialCodec _credentialCodec;
    private readonly OAuthStateCodec _stateCodec;
    private readonly IReadOnlyCollection<IOAuthProvider> _providers;
    private readonly OAuthConnectionOptions _options;

    public OAuthConnectionService(
        AppDbContext dbContext,
        MetaCredentialCodec credentialCodec,
        OAuthStateCodec stateCodec,
        IEnumerable<IOAuthProvider> providers,
        IOptions<OAuthConnectionOptions> options)
    {
        _dbContext = dbContext;
        _credentialCodec = credentialCodec;
        _stateCodec = stateCodec;
        _providers = providers.ToArray();
        _options = options.Value;
    }

    public async Task<IReadOnlyCollection<ConnectionStatusDto>?> GetStatusesAsync(Guid userId, Guid profileId, CancellationToken cancellationToken)
    {
        if (!await OwnsProfileAsync(userId, profileId, cancellationToken)) return null;

        var accounts = await _dbContext.SocialAccounts
            .AsNoTracking()
            .Include(x => x.MetaToken)
            .Where(x => x.ProfileId == profileId)
            .ToListAsync(cancellationToken);

        return Enum.GetValues<SocialPlatform>().Select(platform =>
        {
            var account = accounts.FirstOrDefault(x => x.Platform == platform);
            var provider = FindProvider(platform);
            return new ConnectionStatusDto(
                ToApiName(platform),
                provider is not null,
                provider?.IsConfigured == true,
                account?.MetaToken is { IsDeleted: false } && account.MetaToken.ExpiresAt > DateTimeOffset.UtcNow,
                account?.DisplayName,
                account?.MetaToken?.ExpiresAt);
        }).ToArray();
    }

    public async Task<string?> CreateAuthorizationUrlAsync(Guid userId, Guid profileId, SocialPlatform platform, CancellationToken cancellationToken)
    {
        if (!await OwnsProfileAsync(userId, profileId, cancellationToken)) return null;
        var provider = FindProvider(platform) ?? throw new NotSupportedException("แพลตฟอร์มนี้ยังไม่มี API สำหรับเชื่อมต่ออย่างเป็นทางการ");
        if (!provider.IsConfigured) throw new InvalidOperationException("ยังไม่ได้ตั้งค่า Developer App สำหรับแพลตฟอร์มนี้");

        var verifier = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(48));
        var challenge = WebEncoders.Base64UrlEncode(SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(verifier)));
        var state = _stateCodec.Encode(new OAuthStatePayload(userId, profileId, platform, verifier, DateTimeOffset.UtcNow.AddMinutes(10)));
        return provider.BuildAuthorizationUrl(state, challenge);
    }

    public async Task<OAuthStatePayload> CompleteAsync(
        OAuthCallbackData callback,
        string state,
        CancellationToken cancellationToken)
    {
        var payload = _stateCodec.Decode(state);
        if (!await OwnsProfileAsync(payload.UserId, payload.ProfileId, cancellationToken))
        {
            throw new InvalidOperationException("ไม่พบโปรไฟล์หรือไม่มีสิทธิ์เชื่อมต่อ");
        }

        var provider = FindProvider(payload.Platform) ?? throw new NotSupportedException("แพลตฟอร์มนี้ยังไม่รองรับ");
        var token = await provider.ExchangeCodeAsync(callback, payload.CodeVerifier, cancellationToken);
        var accounts = await provider.GetAccountsAsync(token, cancellationToken);

        foreach (var external in accounts)
        {
            await UpsertAsync(payload.UserId, payload.ProfileId, external, token, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return payload;
    }

    public async Task<bool> DisconnectAsync(Guid userId, Guid profileId, SocialPlatform platform, CancellationToken cancellationToken)
    {
        if (!await OwnsProfileAsync(userId, profileId, cancellationToken)) return false;

        var platforms = platform is SocialPlatform.Facebook or SocialPlatform.Instagram
            ? new[] { SocialPlatform.Facebook, SocialPlatform.Instagram }
            : new[] { platform };
        var accounts = await _dbContext.SocialAccounts
            .Include(x => x.MetaToken)
            .Where(x => x.ProfileId == profileId && platforms.Contains(x.Platform))
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        foreach (var account in accounts)
        {
            account.MetaToken?.SoftDelete(userId, now);
            account.SoftDelete(userId, now);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public string FrontendOrigin => _options.FrontendOrigin.TrimEnd('/');

    public static bool TryParsePlatform(string value, out SocialPlatform platform)
    {
        if (value.Equals("ebay", StringComparison.OrdinalIgnoreCase))
        {
            platform = SocialPlatform.EBay;
            return true;
        }
        if (value.Replace(" ", string.Empty).Equals("tiktokshop", StringComparison.OrdinalIgnoreCase))
        {
            platform = SocialPlatform.TikTokShop;
            return true;
        }
        return Enum.TryParse(value, true, out platform);
    }

    private IOAuthProvider? FindProvider(SocialPlatform platform)
        => platform == SocialPlatform.Instagram
            ? _providers.FirstOrDefault(x => x.Platform == SocialPlatform.Facebook)
            : _providers.FirstOrDefault(x => x.Platform == platform);

    private Task<bool> OwnsProfileAsync(Guid userId, Guid profileId, CancellationToken cancellationToken)
        => _dbContext.Profiles.AsNoTracking().AnyAsync(x => x.Id == profileId && x.OwnerUserId == userId, cancellationToken);

    private async Task UpsertAsync(Guid userId, Guid profileId, OAuthExternalAccount external, OAuthTokenResult token, CancellationToken cancellationToken)
    {
        var account = await _dbContext.SocialAccounts
            .Include(x => x.MetaToken)
            .FirstOrDefaultAsync(x => x.ProfileId == profileId && x.Platform == external.Platform, cancellationToken);
        var instagramId = external.Platform == SocialPlatform.Instagram ? external.ExternalId : null;
        if (account is null)
        {
            account = new SocialAccount(profileId, external.Platform, external.ExternalId, instagramId, external.DisplayName) { CreatedBy = userId };
            _dbContext.SocialAccounts.Add(account);
        }
        else
        {
            account.Update(external.ExternalId, instagramId, external.DisplayName);
        }

        var accessToken = _credentialCodec.Encode(external.AccessToken ?? token.AccessToken);
        var refreshToken = string.IsNullOrWhiteSpace(token.RefreshToken) ? null : _credentialCodec.Encode(token.RefreshToken);
        if (account.MetaToken is null)
        {
            _dbContext.MetaTokens.Add(new MetaToken(account.Id, accessToken, refreshToken, token.ExpiresAt, token.Scope) { CreatedBy = userId });
        }
        else
        {
            account.MetaToken.Update(accessToken, refreshToken, token.ExpiresAt, token.Scope);
        }
    }

    private static string ToApiName(SocialPlatform platform) => platform switch
    {
        SocialPlatform.EBay => "eBay",
        SocialPlatform.TikTokShop => "TikTok Shop",
        _ => platform.ToString()
    };
}
