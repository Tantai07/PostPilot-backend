using FluentAssertions;
using PostPilot.Api.Features.Profiles;
using PostPilot.Domain.Entities;

namespace PostPilot.UnitTests.Profiles;

public sealed class ProfileMappingTests
{
    [Fact]
    public void ToDto_MapsProfileFields()
    {
        var ownerUserId = Guid.NewGuid();
        var profile = new Profile(ownerUserId, "Main Shop", "PostPilot", "FacebookPage");

        var dto = profile.ToDto();

        dto.Id.Should().Be(profile.Id);
        dto.OwnerUserId.Should().Be(ownerUserId);
        dto.Name.Should().Be("Main Shop");
        dto.WebsiteName.Should().Be("PostPilot");
        dto.DefaultTargets.Should().Be("FacebookPage");
        dto.UpdatedAt.Should().Be(profile.CreatedAt);
    }
}
