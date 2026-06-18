using PostPilot.Api.Features.Categories.Commands;
using PostPilot.Api.Features.Categories.Queries;
using PostPilot.Api.Features.Profiles.Commands;
using PostPilot.Api.Features.Profiles.Queries;

namespace PostPilot.Api.Startup;

public static class ProfileServiceCollectionExtensions
{
    public static IServiceCollection AddProfileFeature(this IServiceCollection services)
    {
        services.AddScoped<ProfileQueryExecutor>();
        services.AddScoped<CreateProfileCommandExecutor>();
        services.AddScoped<CategoryQueryExecutor>();
        services.AddScoped<CreateCategoryCommandExecutor>();
        services.AddScoped<UpdateCategoryCommandExecutor>();
        services.AddScoped<DeleteCategoryCommandExecutor>();

        return services;
    }
}