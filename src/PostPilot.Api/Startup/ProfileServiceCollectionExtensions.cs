using PostPilot.Api.Features.Profiles.Commands;
using PostPilot.Api.Features.Profiles.Queries;

namespace PostPilot.Api.Startup;

public static class ProfileServiceCollectionExtensions
{
    public static IServiceCollection AddProfileFeature(this IServiceCollection services)
    {
        services.AddScoped<ProfileQueryExecutor>();
        services.AddScoped<CreateProfileCommandExecutor>();

        return services;
    }
}
