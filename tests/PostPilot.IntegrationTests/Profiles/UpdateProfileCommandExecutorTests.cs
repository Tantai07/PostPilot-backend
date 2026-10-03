using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Profiles.Commands;
using PostPilot.Api.Features.Profiles.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.IntegrationTests.Profiles;

public sealed class UpdateProfileCommandExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_UpdatesOnlyProfileOwnedByCurrentUser()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var dbContext = new AppDbContext(options);
        var ownerUserId = Guid.NewGuid();
        var profile = new Profile(ownerUserId, "Old name", "Old shop", "Facebook");
        dbContext.Profiles.Add(profile);
        await dbContext.SaveChangesAsync();

        var executor = new UpdateProfileCommandExecutor(dbContext);
        var request = new UpdateProfileRequestDto
        {
            Name = "New name",
            WebsiteName = "New shop",
            DefaultTargets = "Instagram,Etsy"
        };

        var denied = await executor.ExecuteAsync(Guid.NewGuid(), profile.Id, request, CancellationToken.None);
        var updated = await executor.ExecuteAsync(ownerUserId, profile.Id, request, CancellationToken.None);

        denied.Should().BeNull();
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New name");
        updated.WebsiteName.Should().Be("New shop");
        updated.DefaultTargets.Should().Be("Instagram,Etsy");
    }
}
