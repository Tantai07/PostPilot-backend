using PostPilot.Api.Features.Media.Commands;
using PostPilot.Api.Features.Media.Storage;

namespace PostPilot.Api.Startup;

public static class MediaServiceCollectionExtensions
{
    public static IServiceCollection AddMediaFeature(this IServiceCollection services)
    {
        services.AddOptions<MediaStorageOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                options.Provider = configuration["POSTPILOT_STORAGE_PROVIDER"]
                    ?? configuration["Storage:Provider"]
                    ?? "Local";
                options.CloudinaryCloudName = configuration["POSTPILOT_CLOUDINARY_CLOUD_NAME"]
                    ?? configuration["Cloudinary:CloudName"]
                    ?? string.Empty;
                options.CloudinaryApiKey = configuration["POSTPILOT_CLOUDINARY_API_KEY"]
                    ?? configuration["Cloudinary:ApiKey"]
                    ?? string.Empty;
                options.CloudinaryApiSecret = configuration["POSTPILOT_CLOUDINARY_API_SECRET"]
                    ?? configuration["Cloudinary:ApiSecret"]
                    ?? string.Empty;
                options.CloudinaryFolder = configuration["POSTPILOT_CLOUDINARY_FOLDER"]
                    ?? configuration["Cloudinary:Folder"]
                    ?? "postpilot";
            });

        services.AddHttpClient<IMediaStorageService, MediaStorageService>();
        services.AddScoped<UploadMediaCommandExecutor>();

        return services;
    }
}