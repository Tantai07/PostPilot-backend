using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Development;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Auth;
using PostPilot.Infrastructure.Database;

namespace PostPilot.IntegrationTests.Auth;

public sealed class DevelopmentUserSeederTests
{
    [Fact]
    public async Task SeedAsync_UpsertsActiveUserAndAdminWithHashedPasswords()
    {
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new AppDbContext(dbOptions);
        var passwordHasher = new Pbkdf2PasswordHasher();
        var seeder = new DevelopmentUserSeeder(dbContext, passwordHasher);
        var options = new DevelopmentUserSeedOptions(
            "user@example.com",
            "test-user-password",
            "admin@example.com",
            "test-admin-password");

        await seeder.SeedAsync(options);
        await seeder.SeedAsync(options);

        var users = await dbContext.Users.OrderBy(x => x.Email).ToListAsync();
        users.Should().HaveCount(2);

        var admin = users.Single(x => x.Email == "admin@example.com");
        admin.Role.Should().Be(UserRole.Admin);
        admin.IsActive.Should().BeTrue();
        admin.RowVersion.Should().HaveCount(16);
        passwordHasher.Verify("test-admin-password", admin.PasswordHash).Should().BeTrue();

        var user = users.Single(x => x.Email == "user@example.com");
        user.Role.Should().Be(UserRole.User);
        user.IsActive.Should().BeTrue();
        user.RowVersion.Should().HaveCount(16);
        passwordHasher.Verify("test-user-password", user.PasswordHash).Should().BeTrue();
    }
}
