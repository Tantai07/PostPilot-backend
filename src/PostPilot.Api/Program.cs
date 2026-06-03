using PostPilot.Api.Shared;
using PostPilot.Api.Startup;
using PostPilot.Infrastructure.Database;
using PostPilot.Infrastructure.Startup;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy(ApiConstants.LocalFrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://127.0.0.1:3000",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddPostPilotInfrastructure(builder.Configuration)
    .AddPostPilotAuth(builder.Configuration)
    .AddProfileFeature();

var app = builder.Build();

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
app.UseCors(ApiConstants.LocalFrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

if (string.Equals(builder.Configuration["POSTPILOT_SEED_ADMIN_ON_STARTUP"], "true", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
    await seeder.SeedAsync();
}

app.Run();
