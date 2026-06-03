using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Infrastructure.Database;

public sealed class AdminUserSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<AdminUserSeeder> logger)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = _configuration["POSTPILOT_SEED_ADMIN_EMAIL"];
        var password = _configuration["POSTPILOT_SEED_ADMIN_PASSWORD"];
        var displayName = _configuration["POSTPILOT_SEED_ADMIN_DISPLAY_NAME"] ?? "PostPilot Admin";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogInformation("Admin seed skipped because POSTPILOT_SEED_ADMIN_EMAIL or POSTPILOT_SEED_ADMIN_PASSWORD is missing.");
            return;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var exists = await _dbContext.AdminUsers.IgnoreQueryFilters().AnyAsync(x => x.Email == normalizedEmail, cancellationToken);
        if (exists)
        {
            return;
        }

        _dbContext.AdminUsers.Add(new AdminUser(normalizedEmail, _passwordHasher.Hash(password), displayName));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
