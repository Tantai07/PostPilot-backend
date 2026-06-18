using PostPilot.Api.Features.Queue.Commands;
using PostPilot.Api.Features.Queue.Queries;

namespace PostPilot.Api.Startup;

public static class QueueServiceCollectionExtensions
{
    public static IServiceCollection AddQueueFeature(this IServiceCollection services)
    {
        services.AddScoped<QueueQueryExecutor>();
        services.AddScoped<AddPostToQueueCommandExecutor>();
        services.AddScoped<ReorderQueueCommandExecutor>();
        services.AddScoped<PostNextQueueCommandExecutor>();

        return services;
    }
}