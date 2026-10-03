using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PostPilot.Api.Features.Connections;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Connections;

public sealed class MarketplaceOAuthProviderTests
{
    [Fact]
    public void LazadaAuthorizationUrl_ContainsAppCallbackAndState()
    {
        var provider = new LazadaOAuthProvider(new HttpClient(new StubHandler(_ => Fail())), Options.Create(CreateOptions()));

        var uri = new Uri(provider.BuildAuthorizationUrl("state", "unused"));
        var query = QueryHelpers.ParseQuery(uri.Query);

        provider.Platform.Should().Be(SocialPlatform.Lazada);
        provider.IsConfigured.Should().BeTrue();
        uri.GetLeftPart(UriPartial.Path).Should().Be("https://auth.lazada.com/oauth/authorize");
        query["client_id"].ToString().Should().Be("lazada-key");
        query["redirect_uri"].ToString().Should().EndWith("/api/oauth/lazada/callback");
        query["state"].ToString().Should().Be("state");
    }

    [Fact]
    public async Task LazadaExchange_MapsSellerAccount()
    {
        var provider = new LazadaOAuthProvider(new HttpClient(new StubHandler(_ => Task.FromResult(Json(
            """{"access_token":"access","refresh_token":"refresh","expires_in":3600,"account":"seller@example.com","country":"th"}""")))), Options.Create(CreateOptions()));

        var token = await provider.ExchangeCodeAsync(Callback("code"), "unused", CancellationToken.None);
        var account = (await provider.GetAccountsAsync(token, CancellationToken.None)).Single();

        account.Should().Be(new OAuthExternalAccount(SocialPlatform.Lazada, "seller@example.com", "seller@example.com (TH)"));
        token.RefreshToken.Should().Be("refresh");
    }

    [Fact]
    public void ShopeeAuthorizationUrl_UsesSignedPartnerEndpointAndProtectedStateInRedirect()
    {
        var provider = new ShopeeOAuthProvider(new HttpClient(new StubHandler(_ => Fail())), Options.Create(CreateOptions()));

        var uri = new Uri(provider.BuildAuthorizationUrl("state", "unused"));
        var query = QueryHelpers.ParseQuery(uri.Query);

        provider.Platform.Should().Be(SocialPlatform.Shopee);
        uri.AbsolutePath.Should().Be("/api/v2/shop/auth_partner");
        query["partner_id"].ToString().Should().Be("123456");
        query["sign"].ToString().Should().MatchRegex("^[a-f0-9]{64}$");
        query["redirect"].ToString().Should().Contain("/api/oauth/shopee/callback").And.Contain("state=state");
    }

    [Fact]
    public async Task ShopeeExchange_RequiresAndMapsShopId()
    {
        var provider = new ShopeeOAuthProvider(new HttpClient(new StubHandler(_ => Task.FromResult(Json(
            """{"access_token":"access","refresh_token":"refresh","expire_in":14400,"error":"","message":""}""")))), Options.Create(CreateOptions()));
        var callback = new OAuthCallbackData("code", new Dictionary<string, string> { ["shop_id"] = "98765" });

        var token = await provider.ExchangeCodeAsync(callback, "unused", CancellationToken.None);
        var account = (await provider.GetAccountsAsync(token, CancellationToken.None)).Single();

        account.Should().Be(new OAuthExternalAccount(SocialPlatform.Shopee, "98765", "Shopee Shop 98765"));
    }

    [Fact]
    public void TikTokShopAuthorizationUrl_PreservesPartnerUrlAndAddsState()
    {
        var provider = new TikTokShopOAuthProvider(new HttpClient(new StubHandler(_ => Fail())), Options.Create(CreateOptions()));

        var uri = new Uri(provider.BuildAuthorizationUrl("state", "unused"));
        var query = QueryHelpers.ParseQuery(uri.Query);

        provider.Platform.Should().Be(SocialPlatform.TikTokShop);
        provider.IsConfigured.Should().BeTrue();
        query["service_id"].ToString().Should().Be("service-id");
        query["state"].ToString().Should().Be("state");
    }

    [Fact]
    public async Task TikTokShopExchange_MapsOpenIdFromNestedTokenResponse()
    {
        var provider = new TikTokShopOAuthProvider(new HttpClient(new StubHandler(_ => Task.FromResult(Json(
            """{"code":0,"message":"Success","data":{"access_token":"access","refresh_token":"refresh","expires_in":3600,"open_id":"seller-open-id","seller_name":"PostPilot Shop"}}""")))), Options.Create(CreateOptions()));

        var token = await provider.ExchangeCodeAsync(Callback("code"), "unused", CancellationToken.None);
        var account = (await provider.GetAccountsAsync(token, CancellationToken.None)).Single();

        account.Should().Be(new OAuthExternalAccount(SocialPlatform.TikTokShop, "seller-open-id", "PostPilot Shop"));
    }

    private static OAuthCallbackData Callback(string code) => new(code, new Dictionary<string, string>());

    private static OAuthConnectionOptions CreateOptions() => new()
    {
        CallbackBaseUrl = "https://api.postpilot.test",
        LazadaAppKey = "lazada-key",
        LazadaAppSecret = "lazada-secret",
        ShopeePartnerId = "123456",
        ShopeePartnerKey = "shopee-key",
        ShopeeApiBaseUrl = "https://partner.shopeemobile.com",
        TikTokShopAppKey = "tiktok-key",
        TikTokShopAppSecret = "tiktok-secret",
        TikTokShopAuthorizationUrl = "https://services.tiktokshop.com/open/authorize?service_id=service-id"
    };

    private static HttpResponseMessage Json(string body) => new(HttpStatusCode.OK)
    {
        Content = new StringContent(body, Encoding.UTF8, "application/json")
    };

    private static Task<HttpResponseMessage> Fail() => throw new InvalidOperationException("HTTP was not expected.");

    private sealed class StubHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => handler(request);
    }
}
