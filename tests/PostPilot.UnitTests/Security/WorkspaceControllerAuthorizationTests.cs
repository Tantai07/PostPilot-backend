using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using PostPilot.Api.Features.Categories;
using PostPilot.Api.Features.Dashboard;
using PostPilot.Api.Features.History;
using PostPilot.Api.Features.Media;
using PostPilot.Api.Features.Meta;
using PostPilot.Api.Features.Posts;
using PostPilot.Api.Features.Queue;

namespace PostPilot.UnitTests.Security;

public sealed class WorkspaceControllerAuthorizationTests
{
    public static TheoryData<Type> WorkspaceControllers => new()
    {
        typeof(DashboardController),
        typeof(PostController),
        typeof(QueueController),
        typeof(PostQueueController),
        typeof(HistoryController),
        typeof(CategoryEndpoints),
        typeof(MediaController),
        typeof(MetaConnectionController)
    };

    [Theory]
    [MemberData(nameof(WorkspaceControllers))]
    public void Controller_RequiresAuthenticationWithoutRolePolicy(Type controllerType)
    {
        var authorizeAttribute = controllerType
            .GetCustomAttributes<AuthorizeAttribute>()
            .Should()
            .ContainSingle()
            .Which;

        authorizeAttribute.Policy.Should().BeNull();
        authorizeAttribute.Roles.Should().BeNull();
    }
}
