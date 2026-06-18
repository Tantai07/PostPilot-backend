using FluentAssertions;
using PostPilot.Api.Features.Categories.Queries;
using PostPilot.Domain.Entities;

namespace PostPilot.UnitTests.Categories;

public sealed class CategoryQueryExtensionTests
{
    [Fact]
    public void ApplyProfileScope_ReturnsOnlyCategoriesForProfile()
    {
        var profileId = Guid.NewGuid();
        var otherProfileId = Guid.NewGuid();
        var categories = new[]
        {
            new Category(profileId, "Shirts", "#111827", null),
            new Category(otherProfileId, "Toys", "#1f2937", null)
        }.AsQueryable();

        var result = categories.ApplyProfileScope(profileId).ToList();

        result.Should().ContainSingle();
        result[0].ProfileId.Should().Be(profileId);
    }

    [Fact]
    public void ApplyKeyword_MatchesNameDescriptionOrTagText()
    {
        var profileId = Guid.NewGuid();
        var shirt = new Category(profileId, "Shirts", "#111827", null);
        var toy = new Category(profileId, "Toys", "#1f2937", "Plush collection");
        var bag = new Category(profileId, "Bags", "#374151", null);
        bag.ReplaceTags(new[] { ("#handmade", 1) }, Guid.NewGuid(), DateTimeOffset.UtcNow);

        var categories = new[] { shirt, toy, bag }.AsQueryable();

        categories.ApplyKeyword("shirt").Should().ContainSingle(x => x.Name == "Shirts");
        categories.ApplyKeyword("Plush").Should().ContainSingle(x => x.Name == "Toys");
        categories.ApplyKeyword("handmade").Should().ContainSingle(x => x.Name == "Bags");
    }

    [Fact]
    public void ApplyDeterministicOrder_OrdersByNameThenId()
    {
        var profileId = Guid.NewGuid();
        var categories = new[]
        {
            new Category(profileId, "Toys", "#111827", null),
            new Category(profileId, "Bags", "#111827", null),
            new Category(profileId, "Shirts", "#111827", null)
        }.AsQueryable();

        var result = categories.ApplyDeterministicOrder().ToList();

        result.Select(x => x.Name).Should().Equal("Bags", "Shirts", "Toys");
    }
}