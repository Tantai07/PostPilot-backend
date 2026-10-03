using PostPilot.Api.Features.Meta.Commands;
using PostPilot.Api.Features.Meta.Queries;
using PostPilot.Api.Features.Meta.Security;
using PostPilot.Api.Features.Connections;
using Microsoft.AspNetCore.DataProtection;

namespace PostPilot.Api.Startup;

public static class MetaServiceCollectionExtensions
{
    public static IServiceCollection AddMetaFeature(this IServiceCollection services, IConfiguration configuration)
    {
        var configuredKeyPath = configuration["POSTPILOT_DATA_PROTECTION_KEYS_PATH"];
        var keyPath = string.IsNullOrWhiteSpace(configuredKeyPath)
            ? Path.Combine(AppContext.BaseDirectory, "data-protection-keys")
            : configuredKeyPath;
        services.AddDataProtection()
            .SetApplicationName("PostPilot")
            .PersistKeysToFileSystem(new DirectoryInfo(keyPath));
        services.AddOptions<OAuthConnectionOptions>().Configure(options =>
        {
            options.FrontendOrigin = configuration["POSTPILOT_FRONTEND_ORIGIN"] ?? "http://localhost:5173";
            options.CallbackBaseUrl = configuration["POSTPILOT_OAUTH_CALLBACK_BASE_URL"] ?? "http://localhost:5270";
            options.MetaClientId = configuration["POSTPILOT_META_CLIENT_ID"] ?? string.Empty;
            options.MetaClientSecret = configuration["POSTPILOT_META_CLIENT_SECRET"] ?? string.Empty;
            options.MetaGraphApiVersion = configuration["POSTPILOT_META_GRAPH_API_VERSION"] ?? "v24.0";
            options.XClientId = configuration["POSTPILOT_X_CLIENT_ID"] ?? string.Empty;
            options.XClientSecret = configuration["POSTPILOT_X_CLIENT_SECRET"] ?? string.Empty;
            options.EbayClientId = configuration["POSTPILOT_EBAY_CLIENT_ID"] ?? string.Empty;
            options.EbayClientSecret = configuration["POSTPILOT_EBAY_CLIENT_SECRET"] ?? string.Empty;
            options.EbayRedirectUri = configuration["POSTPILOT_EBAY_REDIRECT_URI"] ?? string.Empty;
            options.EbaySandbox = !bool.TryParse(configuration["POSTPILOT_EBAY_SANDBOX"], out var sandbox) || sandbox;
            options.EtsyClientId = configuration["POSTPILOT_ETSY_CLIENT_ID"] ?? string.Empty;
            options.EtsySharedSecret = configuration["POSTPILOT_ETSY_SHARED_SECRET"] ?? string.Empty;
            options.LazadaAppKey = configuration["POSTPILOT_LAZADA_APP_KEY"] ?? string.Empty;
            options.LazadaAppSecret = configuration["POSTPILOT_LAZADA_APP_SECRET"] ?? string.Empty;
            options.ShopeePartnerId = configuration["POSTPILOT_SHOPEE_PARTNER_ID"] ?? string.Empty;
            options.ShopeePartnerKey = configuration["POSTPILOT_SHOPEE_PARTNER_KEY"] ?? string.Empty;
            options.ShopeeApiBaseUrl = configuration["POSTPILOT_SHOPEE_API_BASE_URL"] ?? "https://partner.shopeemobile.com";
            options.TikTokShopAppKey = configuration["POSTPILOT_TIKTOK_SHOP_APP_KEY"] ?? string.Empty;
            options.TikTokShopAppSecret = configuration["POSTPILOT_TIKTOK_SHOP_APP_SECRET"] ?? string.Empty;
            options.TikTokShopAuthorizationUrl = configuration["POSTPILOT_TIKTOK_SHOP_AUTHORIZATION_URL"] ?? string.Empty;
        });
        services.AddScoped<MetaCredentialCodec>();
        services.AddSingleton<OAuthStateCodec>();
        services.AddScoped<OAuthConnectionService>();
        services.AddScoped<MetaConnectionQueryExecutor>();
        services.AddScoped<SaveMetaConnectionCommandExecutor>();
        services.AddHttpClient<MetaOAuthProvider>();
        services.AddHttpClient<XOAuthProvider>();
        services.AddHttpClient<EbayOAuthProvider>();
        services.AddHttpClient<EtsyOAuthProvider>();
        services.AddHttpClient<LazadaOAuthProvider>();
        services.AddHttpClient<ShopeeOAuthProvider>();
        services.AddHttpClient<TikTokShopOAuthProvider>();
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<MetaOAuthProvider>());
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<XOAuthProvider>());
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<EbayOAuthProvider>());
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<EtsyOAuthProvider>());
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<LazadaOAuthProvider>());
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<ShopeeOAuthProvider>());
        services.AddTransient<IOAuthProvider>(provider => provider.GetRequiredService<TikTokShopOAuthProvider>());
        return services;
    }
}
