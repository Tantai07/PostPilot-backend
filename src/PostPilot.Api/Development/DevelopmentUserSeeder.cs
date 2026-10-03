using Microsoft.EntityFrameworkCore;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Auth;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Development;

public sealed class DevelopmentUserSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public DevelopmentUserSeeder(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(DevelopmentUserSeedOptions options, CancellationToken cancellationToken = default)
    {
        await UpsertAsync(options.UserEmail, options.UserPassword, "Test User", UserRole.User, cancellationToken);
        await UpsertAsync(options.AdminEmail, options.AdminPassword, "Test Admin", UserRole.Admin, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpsertAsync(
        string email,
        string password,
        string displayName,
        UserRole role,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var passwordHash = _passwordHasher.Hash(password);
        var user = await _dbContext.Users
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);

        if (user is null)
        {
            _dbContext.Users.Add(new User(normalizedEmail, passwordHash, displayName, role));
            return;
        }

        var entry = _dbContext.Entry(user);
        entry.Property<string>(nameof(User.PasswordHash)).CurrentValue = passwordHash;
        entry.Property<string>(nameof(User.DisplayName)).CurrentValue = displayName;
        entry.Property<UserRole>(nameof(User.Role)).CurrentValue = role;
        entry.Property<bool>(nameof(User.IsActive)).CurrentValue = true;
        entry.Property<bool>(nameof(User.IsDeleted)).CurrentValue = false;
        entry.Property<DateTimeOffset?>(nameof(User.DeletedAt)).CurrentValue = null;
        entry.Property<Guid?>(nameof(User.DeletedBy)).CurrentValue = null;
    }
}
