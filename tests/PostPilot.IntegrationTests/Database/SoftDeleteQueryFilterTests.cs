using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.IntegrationTests.Database;

public sealed class SoftDeleteQueryFilterTests
{
    [Fact]
    public async Task Profiles_ExcludeSoftDeletedRows()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new AppDbContext(options);
        var active = new Profile(Guid.NewGuid(), "Active", null, null);
        var deleted = new Profile(Guid.NewGuid(), "Deleted", null, null);
        deleted.SoftDelete(Guid.NewGuid(), DateTimeOffset.UtcNow);

        dbContext.Profiles.AddRange(active, deleted);
        await dbContext.SaveChangesAsync();

        var profiles = await dbContext.Profiles.ToListAsync();

        profiles.Should().ContainSingle();
        profiles[0].Name.Should().Be("Active");
    }
}
