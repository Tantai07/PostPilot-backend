using PostPilot.Api.Features.History.Queries;

namespace PostPilot.Api.Startup;

public static class HistoryServiceCollectionExtensions
{
    public static IServiceCollection AddHistoryFeature(this IServiceCollection services)
    {
        services.AddScoped<HistoryQueryExecutor>();

        return services;
    }
}
