using PostPilot.Api.Features.Posts.Commands;
using PostPilot.Api.Features.Posts.Queries;

namespace PostPilot.Api.Startup;

public static class PostServiceCollectionExtensions
{
    public static IServiceCollection AddPostFeature(this IServiceCollection services)
    {
        services.AddScoped<CreatePostDraftCommandExecutor>();
        services.AddScoped<PublishPostNowCommandExecutor>();
        services.AddScoped<PostListQueryExecutor>();

        return services;
    }
}