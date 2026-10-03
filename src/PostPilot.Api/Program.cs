using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Development;
using PostPilot.Api.Shared;
using PostPilot.Api.Startup;
using PostPilot.Infrastructure.Database;
using PostPilot.Infrastructure.Startup;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<DevelopmentUserSeeder>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.LocalFrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5174",
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5173",
                "http://127.0.0.1:5174")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddPostPilotInfrastructure(builder.Configuration)
    .AddPostPilotAuth(builder.Configuration)
    .AddProfileFeature()
    .AddCategoryFeature()
    .AddMediaFeature()
    .AddPostFeature()
    .AddQueueFeature()
    .AddPublishingFeature()
    .AddHistoryFeature()
    .AddDashboardFeature()
    .AddMetaFeature(builder.Configuration);

var app = builder.Build();

if (args.Contains("--seed-test-users", StringComparer.OrdinalIgnoreCase))
{
    if (!app.Environment.IsDevelopment())
    {
        throw new InvalidOperationException("Test users can only be seeded in the Development environment.");
    }

    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var options = DevelopmentUserSeedOptions.FromConfiguration(app.Configuration);
    var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentUserSeeder>();
    await seeder.SeedAsync(options);
    Console.WriteLine("Development test users seeded successfully.");
    return;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("PostPilot API");
    });
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(ApiConstants.LocalFrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
