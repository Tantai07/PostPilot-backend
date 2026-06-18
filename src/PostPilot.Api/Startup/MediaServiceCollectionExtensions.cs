using PostPilot.Api.Features.Media.Commands;
using PostPilot.Api.Features.Media.Storage;

namespace PostPilot.Api.Startup;

public static class MediaServiceCollectionExtensions
{
    public static IServiceCollection AddMediaFeature(this IServiceCollection services)
    {
        services.AddScoped<UploadMediaCommandExecutor>();
        services.AddScoped<IMediaStorageService, LocalMediaStorageService>();

        return services;
    }
}