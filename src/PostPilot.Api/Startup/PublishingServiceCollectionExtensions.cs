using Microsoft.Extensions.Options;
using PostPilot.Api.Features.Publishing;
using PostPilot.Api.Features.Publishing.Meta;

namespace PostPilot.Api.Startup;

public static class PublishingServiceCollectionExtensions
{
    public static IServiceCollection AddPublishingFeature(this IServiceCollection services)
    {
        services.AddOptions<PublishingOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                options.Provider = configuration["POSTPILOT_PUBLISH_PROVIDER"]
                    ?? configuration["Publishing:Provider"]
                    ?? "Mock";
                options.GraphApiVersion = configuration["POSTPILOT_META_GRAPH_API_VERSION"]
                    ?? configuration["Meta:GraphApiVersion"]
                    ?? "v20.0";
            });

        services.AddScoped<MockPostPublisher>();
        services.AddHttpClient<MetaPostPublisher>();
        services.AddScoped<IPostPublisher>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<PublishingOptions>>().Value;
            return options.Provider.Equals("Meta", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<MetaPostPublisher>()
                : provider.GetRequiredService<MockPostPublisher>();
        });

        return services;
    }
}