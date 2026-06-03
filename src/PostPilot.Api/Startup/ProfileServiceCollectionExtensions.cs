using PostPilot.Api.Features.Profiles;

namespace PostPilot.Api.Startup;

public static class ProfileServiceCollectionExtensions
{
    public static IServiceCollection AddProfileFeature(this IServiceCollection services)
    {
        services.AddScoped<ProfileQueryExecutor>();
        services.AddScoped<CreateProfileCommand>();

        return services;
    }
}
