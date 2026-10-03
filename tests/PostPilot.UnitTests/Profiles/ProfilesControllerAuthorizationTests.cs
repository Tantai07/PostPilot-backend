using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using PostPilot.Api.Features.Profiles;

namespace PostPilot.UnitTests.Profiles;

public sealed class ProfilesControllerAuthorizationTests
{
    [Fact]
    public void Controller_RequiresAuthenticationWithoutRolePolicy()
    {
        var authorizeAttribute = typeof(ProfilesController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .Should()
            .ContainSingle()
            .Which;

        authorizeAttribute.Policy.Should().BeNull();
        authorizeAttribute.Roles.Should().BeNull();
    }
}
