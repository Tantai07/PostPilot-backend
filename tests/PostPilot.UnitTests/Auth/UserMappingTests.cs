using System.Text.Json;
using FluentAssertions;
using PostPilot.Api.Features.Auth;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Auth;

public sealed class UserMappingTests
{
    [Fact]
    public void ToDto_MapsSafeUserFieldsIncludingRole()
    {
        var user = new User("admin@example.com", "hashed-password", "Admin", UserRole.Admin);

        var dto = user.ToDto();

        dto.Id.Should().Be(user.Id);
        dto.Email.Should().Be("admin@example.com");
        dto.DisplayName.Should().Be("Admin");
        dto.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public void UserRole_SerializesAsString()
    {
        var user = new User("admin@example.com", "hashed-password", "Admin", UserRole.Admin);

        var json = JsonSerializer.Serialize(user.ToDto());

        json.Should().Contain("\"Role\":\"Admin\"");
    }
}
