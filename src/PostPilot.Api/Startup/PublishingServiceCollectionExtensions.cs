using PostPilot.Api.Features.Publishing;
using PostPilot.Api.Features.Publishing.Commands;

namespace PostPilot.Api.Startup;

public static class PublishingServiceCollectionExtensions
{
    public static IServiceCollection AddPublishingFeature(this IServiceCollection services)
    {
        services.AddScoped<IPostPublisher, MockPostPublisher>();
        services.AddScoped<PublishPostCommandExecutor>();

        return services;
    }
}