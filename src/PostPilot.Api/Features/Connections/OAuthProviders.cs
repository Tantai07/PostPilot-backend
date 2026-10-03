using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.Connections;

public sealed record OAuthTokenResult(
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset ExpiresAt,
    string? Scope,
    string? ExternalId = null,
    string? DisplayName = null);

public sealed record OAuthCallbackData(
    string Code,
    IReadOnlyDictionary<string, string> Parameters);

public sealed record OAuthExternalAccount(
    SocialPlatform Platform,
    string ExternalId,
    string DisplayName,
    string? AccessToken = null);

public interface IOAuthProvider
{
    SocialPlatform Platform { get; }
    bool IsConfigured { get; }
    string BuildAuthorizationUrl(string state, string codeChallenge);
    Task<OAuthTokenResult> ExchangeCodeAsync(OAuthCallbackData callback, string codeVerifier, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(OAuthTokenResult token, CancellationToken cancellationToken);
}

public abstract class OAuthProviderBase : IOAuthProvider
{
    protected OAuthProviderBase(HttpClient httpClient, IOptions<OAuthConnectionOptions> options)
    {
        HttpClient = httpClient;
        Options = options.Value;
    }

    protected HttpClient HttpClient { get; }
    protected OAuthConnectionOptions Options { get; }
    public abstract SocialPlatform Platform { get; }
    public abstract bool IsConfigured { get; }
    public abstract string BuildAuthorizationUrl(string state, string codeChallenge);
    public abstract Task<OAuthTokenResult> ExchangeCodeAsync(OAuthCallbackData callback, string codeVerifier, CancellationToken cancellationToken);
    public abstract Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(OAuthTokenResult token, CancellationToken cancellationToken);

    protected string CallbackUrl => $"{Options.CallbackBaseUrl.TrimEnd('/')}/api/oauth/{Platform.ToString().ToLowerInvariant()}/callback";

    protected static async Task<JsonNode> ReadJsonAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OAuth provider rejected the request ({(int)response.StatusCode}).");
        }

        return JsonNode.Parse(body) ?? throw new InvalidOperationException("OAuth provider returned an empty response.");
    }

    protected static OAuthTokenResult ParseToken(JsonNode json)
    {
        var accessToken = json["access_token"]?.GetValue<string>()
            ?? throw new InvalidOperationException("OAuth provider did not return an access token.");
        var expiresIn = json["expires_in"]?.GetValue<int?>()
            ?? json["expire_in"]?.GetValue<int?>()
            ?? 3600;
        return new OAuthTokenResult(
            accessToken,
            json["refresh_token"]?.GetValue<string>(),
            DateTimeOffset.UtcNow.AddSeconds(expiresIn),
            json["scope"]?.GetValue<string>());
    }
}

public sealed class MetaOAuthProvider : OAuthProviderBase
{
    public MetaOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.Facebook;
    public override bool IsConfigured => !string.IsNullOrWhiteSpace(Options.MetaClientId) && !string.IsNullOrWhiteSpace(Options.MetaClientSecret);

    public override string BuildAuthorizationUrl(string state, string codeChallenge) => QueryHelpers.AddQueryString(
        $"https://www.facebook.com/{Options.MetaGraphApiVersion}/dialog/oauth",
        new Dictionary<string, string?>
        {
            ["client_id"] = Options.MetaClientId,
            ["redirect_uri"] = CallbackUrl,
            ["response_type"] = "code",
            ["state"] = state,
            ["scope"] = "pages_show_list,pages_read_engagement,pages_manage_posts,instagram_basic,instagram_content_publish"
        });

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(OAuthCallbackData callback, string codeVerifier, CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString($"https://graph.facebook.com/{Options.MetaGraphApiVersion}/oauth/access_token", new Dictionary<string, string?>
        {
            ["client_id"] = Options.MetaClientId,
            ["client_secret"] = Options.MetaClientSecret,
            ["redirect_uri"] = CallbackUrl,
            ["code"] = callback.Code
        });
        return ParseToken(await ReadJsonAsync(await HttpClient.GetAsync(url, cancellationToken), cancellationToken));
    }

    public override async Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(OAuthTokenResult token, CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString($"https://graph.facebook.com/{Options.MetaGraphApiVersion}/me/accounts", new Dictionary<string, string?>
        {
            ["fields"] = "id,name,access_token,instagram_business_account{id,username}",
            ["access_token"] = token.AccessToken
        });
        var json = await ReadJsonAsync(await HttpClient.GetAsync(url, cancellationToken), cancellationToken);
        var accounts = new List<OAuthExternalAccount>();
        foreach (var page in json["data"]?.AsArray() ?? [])
        {
            if (page is null) continue;
            var pageId = page["id"]?.GetValue<string>();
            var pageName = page["name"]?.GetValue<string>();
            var pageToken = page["access_token"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(pageId) && !string.IsNullOrWhiteSpace(pageName))
            {
                accounts.Add(new OAuthExternalAccount(SocialPlatform.Facebook, pageId, pageName, pageToken));
            }

            var instagram = page["instagram_business_account"];
            var instagramId = instagram?["id"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(instagramId))
            {
                accounts.Add(new OAuthExternalAccount(
                    SocialPlatform.Instagram,
                    instagramId,
                    instagram?["username"]?.GetValue<string>() ?? $"Instagram {instagramId}",
                    pageToken));
            }
        }

        if (accounts.Count == 0)
        {
            throw new InvalidOperationException("ไม่พบ Facebook Page หรือ Instagram Professional ที่บัญชีนี้ดูแล");
        }

        return accounts;
    }
}

public sealed class XOAuthProvider : OAuthProviderBase
{
    public XOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.X;
    public override bool IsConfigured => !string.IsNullOrWhiteSpace(Options.XClientId);
    public override string BuildAuthorizationUrl(string state, string codeChallenge) => QueryHelpers.AddQueryString(
        "https://twitter.com/i/oauth2/authorize",
        new Dictionary<string, string?>
        {
            ["response_type"] = "code", ["client_id"] = Options.XClientId, ["redirect_uri"] = CallbackUrl,
            ["scope"] = "tweet.read tweet.write users.read offline.access", ["state"] = state,
            ["code_challenge"] = codeChallenge, ["code_challenge_method"] = "S256"
        });

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(OAuthCallbackData callback, string codeVerifier, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.x.com/2/oauth2/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code", ["code"] = callback.Code, ["redirect_uri"] = CallbackUrl,
                ["client_id"] = Options.XClientId, ["code_verifier"] = codeVerifier
            })
        };
        if (!string.IsNullOrWhiteSpace(Options.XClientSecret))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Options.XClientId}:{Options.XClientSecret}")));
        }
        return ParseToken(await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken));
    }

    public override async Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(OAuthTokenResult token, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.x.com/2/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        var data = (await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken))["data"]!;
        return [new OAuthExternalAccount(Platform, data["id"]!.GetValue<string>(), data["username"]?.GetValue<string>() ?? data["name"]!.GetValue<string>())];
    }
}

public sealed class EbayOAuthProvider : OAuthProviderBase
{
    public EbayOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.EBay;
    public override bool IsConfigured => !string.IsNullOrWhiteSpace(Options.EbayClientId) && !string.IsNullOrWhiteSpace(Options.EbayClientSecret) && !string.IsNullOrWhiteSpace(Options.EbayRedirectUri);
    private string ApiHost => Options.EbaySandbox ? "api.sandbox.ebay.com" : "api.ebay.com";
    private string AuthHost => Options.EbaySandbox ? "auth.sandbox.ebay.com" : "auth.ebay.com";

    public override string BuildAuthorizationUrl(string state, string codeChallenge) => QueryHelpers.AddQueryString(
        $"https://{AuthHost}/oauth2/authorize",
        new Dictionary<string, string?>
        {
            ["client_id"] = Options.EbayClientId, ["redirect_uri"] = Options.EbayRedirectUri, ["response_type"] = "code",
            ["scope"] = "https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/commerce.identity.readonly",
            ["state"] = state
        });

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(OAuthCallbackData callback, string codeVerifier, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://{ApiHost}/identity/v1/oauth2/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["grant_type"] = "authorization_code", ["code"] = callback.Code, ["redirect_uri"] = Options.EbayRedirectUri })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Options.EbayClientId}:{Options.EbayClientSecret}")));
        return ParseToken(await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken));
    }

    public override async Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(OAuthTokenResult token, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://{ApiHost}/commerce/identity/v1/user/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        var json = await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken);
        var id = json["userId"]?.GetValue<string>() ?? json["username"]?.GetValue<string>() ?? "ebay-user";
        return [new OAuthExternalAccount(Platform, id, json["username"]?.GetValue<string>() ?? id)];
    }
}

public sealed class EtsyOAuthProvider : OAuthProviderBase
{
    public EtsyOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.Etsy;
    public override bool IsConfigured => !string.IsNullOrWhiteSpace(Options.EtsyClientId) && !string.IsNullOrWhiteSpace(Options.EtsySharedSecret);
    public override string BuildAuthorizationUrl(string state, string codeChallenge) => QueryHelpers.AddQueryString(
        "https://www.etsy.com/oauth/connect",
        new Dictionary<string, string?>
        {
            ["response_type"] = "code", ["client_id"] = Options.EtsyClientId, ["redirect_uri"] = CallbackUrl,
            ["scope"] = "profile_r shops_r listings_r listings_w", ["state"] = state,
            ["code_challenge"] = codeChallenge, ["code_challenge_method"] = "S256"
        });

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(OAuthCallbackData callback, string codeVerifier, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.etsy.com/v3/public/oauth/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code", ["client_id"] = Options.EtsyClientId, ["redirect_uri"] = CallbackUrl,
                ["code"] = callback.Code, ["code_verifier"] = codeVerifier
            })
        };
        return ParseToken(await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken));
    }

    public override async Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(OAuthTokenResult token, CancellationToken cancellationToken)
    {
        var userId = token.AccessToken.Split('.', 2)[0];
        using var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.etsy.com/v3/application/users/{Uri.EscapeDataString(userId)}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        request.Headers.Add("x-api-key", $"{Options.EtsyClientId}:{Options.EtsySharedSecret}");
        var json = await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken);
        var displayName = json["login_name"]?.GetValue<string>() ?? json["first_name"]?.GetValue<string>() ?? $"Etsy {userId}";
        return [new OAuthExternalAccount(Platform, userId, displayName)];
    }
}

public sealed class LazadaOAuthProvider : OAuthProviderBase
{
    public LazadaOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.Lazada;
    public override bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Options.LazadaAppKey) &&
        !string.IsNullOrWhiteSpace(Options.LazadaAppSecret);

    public override string BuildAuthorizationUrl(string state, string codeChallenge) => QueryHelpers.AddQueryString(
        "https://auth.lazada.com/oauth/authorize",
        new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["force_auth"] = "true",
            ["client_id"] = Options.LazadaAppKey,
            ["redirect_uri"] = CallbackUrl,
            ["state"] = state
        });

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(
        OAuthCallbackData callback,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        const string apiPath = "/auth/token/create";
        var parameters = new Dictionary<string, string>
        {
            ["app_key"] = Options.LazadaAppKey,
            ["code"] = callback.Code,
            ["sign_method"] = "sha256",
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };
        parameters["sign"] = SignLazada(apiPath, parameters, Options.LazadaAppSecret);
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://auth.lazada.com/rest{apiPath}")
        {
            Content = new FormUrlEncodedContent(parameters)
        };
        var json = await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken);
        var token = ParseToken(json);
        var account = json["account_id"]?.ToString() ?? json["account"]?.GetValue<string>();
        var country = json["country"]?.GetValue<string>();
        return token with
        {
            ExternalId = account ?? throw new InvalidOperationException("Lazada did not return a seller account ID."),
            DisplayName = string.IsNullOrWhiteSpace(country) ? account : $"{account} ({country.ToUpperInvariant()})"
        };
    }

    public override Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(
        OAuthTokenResult token,
        CancellationToken cancellationToken)
    {
        var id = token.ExternalId ?? throw new InvalidOperationException("Lazada did not return a seller account ID.");
        return Task.FromResult<IReadOnlyCollection<OAuthExternalAccount>>(
            [new OAuthExternalAccount(Platform, id, token.DisplayName ?? id)]);
    }

    private static string SignLazada(string apiPath, IReadOnlyDictionary<string, string> parameters, string secret)
    {
        var payload = apiPath + string.Concat(parameters.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => x.Key + x.Value));
        return Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(payload)));
    }
}

public sealed class ShopeeOAuthProvider : OAuthProviderBase
{
    public ShopeeOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.Shopee;
    public override bool IsConfigured =>
        long.TryParse(Options.ShopeePartnerId, out _) && !string.IsNullOrWhiteSpace(Options.ShopeePartnerKey);

    public override string BuildAuthorizationUrl(string state, string codeChallenge)
    {
        const string path = "/api/v2/shop/auth_partner";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var redirect = QueryHelpers.AddQueryString(CallbackUrl, "state", state);
        return QueryHelpers.AddQueryString($"{Options.ShopeeApiBaseUrl.TrimEnd('/')}{path}", new Dictionary<string, string?>
        {
            ["partner_id"] = Options.ShopeePartnerId,
            ["timestamp"] = timestamp.ToString(),
            ["sign"] = Sign(path, timestamp),
            ["redirect"] = redirect
        });
    }

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(
        OAuthCallbackData callback,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        if (!callback.Parameters.TryGetValue("shop_id", out var shopId) || string.IsNullOrWhiteSpace(shopId))
        {
            throw new InvalidOperationException("Shopee did not return a shop_id.");
        }

        const string path = "/api/v2/auth/token/get";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var url = QueryHelpers.AddQueryString($"{Options.ShopeeApiBaseUrl.TrimEnd('/')}{path}", new Dictionary<string, string?>
        {
            ["partner_id"] = Options.ShopeePartnerId,
            ["timestamp"] = timestamp.ToString(),
            ["sign"] = Sign(path, timestamp)
        });
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(new
            {
                partner_id = long.Parse(Options.ShopeePartnerId),
                code = callback.Code,
                shop_id = long.Parse(shopId)
            })
        };
        var json = await ReadJsonAsync(await HttpClient.SendAsync(request, cancellationToken), cancellationToken);
        if (!string.IsNullOrWhiteSpace(json["error"]?.GetValue<string>()))
        {
            throw new InvalidOperationException(json["message"]?.GetValue<string>() ?? "Shopee rejected the authorization code.");
        }
        var token = ParseToken(json);
        return token with { ExternalId = shopId, DisplayName = $"Shopee Shop {shopId}" };
    }

    public override Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(
        OAuthTokenResult token,
        CancellationToken cancellationToken)
    {
        var id = token.ExternalId ?? throw new InvalidOperationException("Shopee shop ID is missing.");
        return Task.FromResult<IReadOnlyCollection<OAuthExternalAccount>>(
            [new OAuthExternalAccount(Platform, id, token.DisplayName ?? id)]);
    }

    private string Sign(string path, long timestamp)
    {
        var payload = $"{Options.ShopeePartnerId}{path}{timestamp}";
        return Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(Options.ShopeePartnerKey),
            Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
    }
}

public sealed class TikTokShopOAuthProvider : OAuthProviderBase
{
    public TikTokShopOAuthProvider(HttpClient client, IOptions<OAuthConnectionOptions> options) : base(client, options) { }
    public override SocialPlatform Platform => SocialPlatform.TikTokShop;
    public override bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Options.TikTokShopAppKey) &&
        !string.IsNullOrWhiteSpace(Options.TikTokShopAppSecret) &&
        Uri.TryCreate(Options.TikTokShopAuthorizationUrl, UriKind.Absolute, out _);

    public override string BuildAuthorizationUrl(string state, string codeChallenge)
        => QueryHelpers.AddQueryString(Options.TikTokShopAuthorizationUrl, "state", state);

    public override async Task<OAuthTokenResult> ExchangeCodeAsync(
        OAuthCallbackData callback,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString("https://auth.tiktok-shops.com/api/v2/token/get", new Dictionary<string, string?>
        {
            ["app_key"] = Options.TikTokShopAppKey,
            ["app_secret"] = Options.TikTokShopAppSecret,
            ["auth_code"] = callback.Code,
            ["grant_type"] = "authorized_code"
        });
        var root = await ReadJsonAsync(await HttpClient.GetAsync(url, cancellationToken), cancellationToken);
        if (root["code"]?.GetValue<int?>() is int resultCode && resultCode != 0)
        {
            throw new InvalidOperationException(root["message"]?.GetValue<string>() ?? "TikTok Shop rejected the authorization code.");
        }
        var json = root["data"] ?? root;
        var token = ParseToken(json);
        var openId = json["open_id"]?.GetValue<string>() ?? json["seller_name"]?.GetValue<string>();
        return token with
        {
            ExternalId = openId ?? throw new InvalidOperationException("TikTok Shop did not return an account ID."),
            DisplayName = json["seller_name"]?.GetValue<string>() ?? openId
        };
    }

    public override Task<IReadOnlyCollection<OAuthExternalAccount>> GetAccountsAsync(
        OAuthTokenResult token,
        CancellationToken cancellationToken)
    {
        var id = token.ExternalId ?? throw new InvalidOperationException("TikTok Shop did not return an account ID.");
        return Task.FromResult<IReadOnlyCollection<OAuthExternalAccount>>(
            [new OAuthExternalAccount(Platform, id, token.DisplayName ?? id)]);
    }
}
