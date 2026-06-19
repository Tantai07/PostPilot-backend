using PostPilot.Api.Features.Meta.Commands;
using PostPilot.Api.Features.Meta.Queries;
using PostPilot.Api.Features.Meta.Security;

namespace PostPilot.Api.Startup;

public static class MetaServiceCollectionExtensions
{
    public static IServiceCollection AddMetaFeature(this IServiceCollection services)
    {
        services.AddDataProtection();
        services.AddScoped<MetaCredentialCodec>();
        services.AddScoped<MetaConnectionQueryExecutor>();
        services.AddScoped<SaveMetaConnectionCommandExecutor>();
        return services;
    }
}
