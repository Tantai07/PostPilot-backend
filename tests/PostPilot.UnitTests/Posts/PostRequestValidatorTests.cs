using FluentAssertions;
using PostPilot.Api.Features.Posts;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.Posts;

public sealed class PostRequestValidatorTests
{
    [Fact]
    public void TryValidateDraftInput_ReturnsFalse_WhenCaptionIsMissing()
    {
        var valid = PostRequestValidator.TryValidateDraftInput(" ", [], [], out var errors);

        valid.Should().BeFalse();
        errors.Should().Contain("Caption is required.");
    }

    [Fact]
    public void TryValidateDraftInput_ReturnsFalse_WhenTargetPlatformIsUnsupported()
    {
        var valid = PostRequestValidator.TryValidateDraftInput("Caption", [], ["Threads"], out var errors);

        valid.Should().BeFalse();
        errors.Should().Contain("Unsupported target platform: Threads.");
    }

    [Theory]
    [InlineData("Facebook Page", PostTargetPlatform.FacebookPage)]
    [InlineData("Instagram Feed", PostTargetPlatform.InstagramFeed)]
    [InlineData("Instagram Story", PostTargetPlatform.InstagramStory)]
    public void TryParseTargetPlatform_ParsesSupportedPlatform(string input, PostTargetPlatform expected)
    {
        var valid = PostRequestValidator.TryParseTargetPlatform(input, out var platform);

        valid.Should().BeTrue();
        platform.Should().Be(expected);
    }

    [Fact]
    public void ParseTargetPlatforms_RemovesDuplicates()
    {
        var result = PostRequestValidator.ParseTargetPlatforms(["Facebook Page", "facebookpage", "Instagram Feed"]);

        result.Should().Equal(PostTargetPlatform.FacebookPage, PostTargetPlatform.InstagramFeed);
    }
}