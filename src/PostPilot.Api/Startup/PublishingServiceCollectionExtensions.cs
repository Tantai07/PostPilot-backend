using PostPilot.Api.Features.Publishing;

namespace PostPilot.Api.Startup;

public static class PublishingServiceCollectionExtensions
{
    public static IServiceCollection AddPublishingFeature(this IServiceCollection services)
    {
        services.AddScoped<IPostPublisher, MockPostPublisher>();

        return services;
    }
}
