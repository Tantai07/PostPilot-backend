using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostPilot.Api.Shared;
using PostPilot.Api.Startup;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Auth;

public sealed class AuthorizationPolicyTests
{
    [Fact]
    public async Task AdminOnlyPolicy_RequiresAdminRole()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddPostPilotAuth(configuration);

        var provider = services.BuildServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();
        var policy = await policyProvider.GetPolicyAsync(AuthorizationPolicies.AdminOnly);

        policy.Should().NotBeNull();
        policy!.Requirements.OfType<RolesAuthorizationRequirement>()
            .Should()
            .ContainSingle()
            .Which.AllowedRoles.Should()
            .ContainSingle(UserRole.Admin.ToString());
    }
}
