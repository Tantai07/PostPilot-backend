using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostPilot.Infrastructure.Auth;
using PostPilot.Infrastructure.Database;
using PostPilot.Infrastructure.Storage;

namespace PostPilot.Infrastructure.Startup;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddPostPilotInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("PostPilotDb");
        var connectionString = !string.IsNullOrWhiteSpace(configuredConnectionString)
            ? configuredConnectionString
            : configuration["POSTPILOT_DATABASE_CONNECTION"]
                ?? "Host=localhost;Port=5432;Database=postpilot;Username=postgres;Password=postgres";

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ITokenEncryptionService, DevelopmentTokenEncryptionService>();
        return services;
    }
}
