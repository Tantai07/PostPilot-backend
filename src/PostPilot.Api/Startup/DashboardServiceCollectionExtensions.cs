using PostPilot.Api.Features.Dashboard.Queries;

namespace PostPilot.Api.Startup;

public static class DashboardServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardFeature(this IServiceCollection services)
    {
        services.AddScoped<DashboardQueryExecutor>();

        return services;
    }
}
