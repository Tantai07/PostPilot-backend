using PostPilot.Api.Features.Posts.Commands;

namespace PostPilot.Api.Startup;

public static class PostServiceCollectionExtensions
{
    public static IServiceCollection AddPostFeature(this IServiceCollection services)
    {
        services.AddScoped<CreatePostDraftCommandExecutor>();
        services.AddScoped<PublishPostNowCommandExecutor>();

        return services;
    }
}