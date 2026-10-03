namespace PostPilot.Api.Features.Connections;

public sealed class OAuthConnectionOptions
{
    public string FrontendOrigin { get; set; } = "http://localhost:5173";
    public string CallbackBaseUrl { get; set; } = "http://localhost:5270";
    public string MetaClientId { get; set; } = string.Empty;
    public string MetaClientSecret { get; set; } = string.Empty;
    public string MetaGraphApiVersion { get; set; } = "v24.0";
    public string XClientId { get; set; } = string.Empty;
    public string XClientSecret { get; set; } = string.Empty;
    public string EbayClientId { get; set; } = string.Empty;
    public string EbayClientSecret { get; set; } = string.Empty;
    public string EbayRedirectUri { get; set; } = string.Empty;
    public bool EbaySandbox { get; set; } = true;
    public string EtsyClientId { get; set; } = string.Empty;
    public string EtsySharedSecret { get; set; } = string.Empty;
    public string LazadaAppKey { get; set; } = string.Empty;
    public string LazadaAppSecret { get; set; } = string.Empty;
    public string ShopeePartnerId { get; set; } = string.Empty;
    public string ShopeePartnerKey { get; set; } = string.Empty;
    public string ShopeeApiBaseUrl { get; set; } = "https://partner.shopeemobile.com";
    public string TikTokShopAppKey { get; set; } = string.Empty;
    public string TikTokShopAppSecret { get; set; } = string.Empty;
    public string TikTokShopAuthorizationUrl { get; set; } = string.Empty;
}
