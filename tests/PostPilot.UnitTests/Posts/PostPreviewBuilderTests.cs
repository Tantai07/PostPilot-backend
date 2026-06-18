using FluentAssertions;
using PostPilot.Api.Features.Posts;

namespace PostPilot.UnitTests.Posts;

public sealed class PostPreviewBuilderTests
{
    [Fact]
    public void BuildCaption_ReturnsTrimmedCaption_WhenNoTags()
    {
        var result = PostPreviewBuilder.BuildCaption("  New product  ", []);

        result.Should().Be("New product");
    }

    [Fact]
    public void BuildCaption_AppendsCategoryTagsOnSeparateLine()
    {
        var result = PostPreviewBuilder.BuildCaption("New product", ["#vintage", "@shop"]);

        result.Should().Be($"New product{Environment.NewLine}{Environment.NewLine}#vintage @shop");
    }

    [Fact]
    public void BuildCaption_RemovesDuplicateTagsIgnoringCase()
    {
        var result = PostPreviewBuilder.BuildCaption("New product", ["#Sale", "#sale", "#new"]);

        result.Should().Be($"New product{Environment.NewLine}{Environment.NewLine}#Sale #new");
    }
}