using PostPilot.Api.Features.Categories.Commands;
using PostPilot.Api.Features.Categories.Queries;

namespace PostPilot.Api.Startup;

public static class CategoryServiceCollectionExtensions
{
    public static IServiceCollection AddCategoryFeature(this IServiceCollection services)
    {
        services.AddScoped<CategoryQueryExecutor>();
        services.AddScoped<CreateCategoryCommandExecutor>();
        services.AddScoped<UpdateCategoryCommandExecutor>();
        services.AddScoped<DeleteCategoryCommandExecutor>();

        return services;
    }
}
