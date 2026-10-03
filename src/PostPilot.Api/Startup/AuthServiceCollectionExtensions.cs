using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PostPilot.Api.Features.Auth;
using PostPilot.Api.Features.Auth.Queries;
using PostPilot.Api.Features.Shared;
using PostPilot.Api.Shared;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Api.Startup;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddPostPilotAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var signingKey = configuration["POSTPILOT_JWT_SIGNING_KEY"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("JWT signing key is not configured. Set POSTPILOT_JWT_SIGNING_KEY.");
        }

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = configuration["POSTPILOT_JWT_ISSUER"] ?? "PostPilot";
            options.Audience = configuration["POSTPILOT_JWT_AUDIENCE"] ?? "PostPilot";
            options.SigningKey = signingKey;
            options.ExpirationMinutes = int.TryParse(configuration["POSTPILOT_JWT_EXPIRATION_MINUTES"], out var minutes)
                ? minutes
                : 120;
        });

        var issuer = configuration["POSTPILOT_JWT_ISSUER"] ?? "PostPilot";
        var audience = configuration["POSTPILOT_JWT_AUDIENCE"] ?? "PostPilot";

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrWhiteSpace(context.Token)
                            && context.Request.Cookies.TryGetValue(AuthCookie.Name, out var accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(UserRole.Admin.ToString());
            });
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<LoginCommand>();
        services.AddScoped<CurrentSessionQuery>();

        return services;
    }
}
