using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PostPilot.Api.Features.Auth;

namespace PostPilot.UnitTests.Auth;

public sealed class AuthCookieTests
{
    [Theory]
    [InlineData("Development", SameSiteMode.Strict, false)]
    [InlineData("Production", SameSiteMode.None, true)]
    public void CreateOptions_UsesEnvironmentAppropriateCrossSitePolicy(
        string environmentName,
        SameSiteMode expectedSameSite,
        bool expectedSecure)
    {
        var environment = new TestWebHostEnvironment { EnvironmentName = environmentName };

        var options = AuthCookie.CreateOptions(DateTimeOffset.UtcNow.AddHours(1), environment);

        options.HttpOnly.Should().BeTrue();
        options.SameSite.Should().Be(expectedSameSite);
        options.Secure.Should().Be(expectedSecure);
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "PostPilot.UnitTests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Development;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
    }
}
