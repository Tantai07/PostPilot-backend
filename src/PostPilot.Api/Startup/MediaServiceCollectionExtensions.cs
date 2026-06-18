using PostPilot.Api.Features.Media.Commands;

namespace PostPilot.Api.Startup;

public static class MediaServiceCollectionExtensions
{
    public static IServiceCollection AddMediaFeature(this IServiceCollection services)
    {
        services.AddScoped<UploadMediaCommandExecutor>();

        return services;
    }
}