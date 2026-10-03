using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Profiles.Commands;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.IntegrationTests.Profiles;

public sealed class DeleteProfileCommandExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_SoftDeletesOnlyOwnedProfileAndStoresDeletionMetadata()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new AppDbContext(options);
        var ownerId = Guid.NewGuid();
        var profile = new Profile(ownerId, "Shop", null, null);
        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();
        var executor = new DeleteProfileCommandExecutor(dbContext);

        (await executor.ExecuteAsync(Guid.NewGuid(), profile.Id, CancellationToken.None)).Should().BeFalse();
        (await executor.ExecuteAsync(ownerId, profile.Id, CancellationToken.None)).Should().BeTrue();

        (await dbContext.Profiles.CountAsync()).Should().Be(0);
        var deleted = await dbContext.Profiles.IgnoreQueryFilters().SingleAsync();
        deleted.IsDeleted.Should().BeTrue();
        deleted.DeletedAt.Should().NotBeNull();
        deleted.DeletedBy.Should().Be(ownerId);
    }
}
