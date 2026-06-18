using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PostPilot.Api.Features.Media;

namespace PostPilot.UnitTests.Media;

public sealed class MediaUploadValidatorTests
{
    [Fact]
    public void TryValidate_ReturnsFalse_WhenFileIsMissing()
    {
        var valid = MediaUploadValidator.TryValidate(null, out var errorMessage);

        valid.Should().BeFalse();
        errorMessage.Should().Be("Image file is required.");
    }

    [Fact]
    public void TryValidate_ReturnsFalse_WhenContentTypeIsUnsupported()
    {
        var file = CreateFile("document.pdf", "application/pdf", 128);

        var valid = MediaUploadValidator.TryValidate(file, out var errorMessage);

        valid.Should().BeFalse();
        errorMessage.Should().Be("Only JPEG, PNG, and WebP images are supported.");
    }

    [Theory]
    [InlineData("image/jpeg", ".jpg")]
    [InlineData("image/png", ".png")]
    [InlineData("image/webp", ".webp")]
    public void GetSafeExtension_ReturnsExpectedExtension(string contentType, string expectedExtension)
    {
        MediaUploadValidator.GetSafeExtension(contentType).Should().Be(expectedExtension);
    }

    [Fact]
    public void TryValidate_ReturnsTrue_ForSupportedImage()
    {
        var file = CreateFile("product.png", "image/png", 128);

        var valid = MediaUploadValidator.TryValidate(file, out var errorMessage);

        valid.Should().BeTrue();
        errorMessage.Should().BeNull();
    }

    private static IFormFile CreateFile(string fileName, string contentType, long size)
    {
        var stream = new MemoryStream(new byte[size]);
        return new FormFile(stream, 0, size, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}