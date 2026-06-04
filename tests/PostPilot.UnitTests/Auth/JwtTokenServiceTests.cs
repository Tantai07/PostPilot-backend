using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using PostPilot.Api.Features.Auth;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Auth;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void CreateLoginResponse_IncludesUserRoleClaim()
    {
        var service = new JwtTokenService(Options.Create(new JwtOptions
        {
            Issuer = "PostPilot.Tests",
            Audience = "PostPilot.Tests",
            SigningKey = "test-signing-key-with-at-least-32-chars",
            ExpirationMinutes = 120
        }));
        var user = new User("admin@example.com", "hashed-password", "Admin", UserRole.Admin);

        var response = service.CreateLoginResponse(user);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        token.Claims.Should().Contain(x => x.Type == ClaimTypes.Role && x.Value == UserRole.Admin.ToString());
        response.User.Role.Should().Be(UserRole.Admin);
    }
}
