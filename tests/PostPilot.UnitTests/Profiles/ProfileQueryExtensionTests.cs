using FluentAssertions;
using PostPilot.Api.Features.Profiles;
using PostPilot.Domain.Entities;

namespace PostPilot.UnitTests.Profiles;

public sealed class ProfileQueryExtensionTests
{
    [Fact]
    public void ApplyOwnerScope_ReturnsOnlyProfilesForOwner()
    {
        var ownerUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profiles = new[]
        {
            new Profile(ownerUserId, "Alpha", null, null),
            new Profile(otherUserId, "Beta", null, null)
        }.AsQueryable();

        var result = profiles.ApplyOwnerScope(ownerUserId).ToList();

        result.Should().ContainSingle();
        result[0].OwnerUserId.Should().Be(ownerUserId);
    }

    [Fact]
    public void ApplyDeterministicOrder_OrdersByNameWebsiteAndId()
    {
        var ownerUserId = Guid.NewGuid();
        var profiles = new[]
        {
            new Profile(ownerUserId, "Beta", null, null),
            new Profile(ownerUserId, "Alpha", "B", null),
            new Profile(ownerUserId, "Alpha", "A", null)
        }.AsQueryable();

        var result = profiles.ApplyDeterministicOrder().ToList();

        result.Select(x => x.Name).Should().Equal("Alpha", "Alpha", "Beta");
        result[0].WebsiteName.Should().Be("A");
        result[1].WebsiteName.Should().Be("B");
    }
}
