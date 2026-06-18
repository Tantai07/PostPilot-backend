using FluentAssertions;
using PostPilot.Domain.Entities;

namespace PostPilot.UnitTests.Categories;

public sealed class CategoryDomainTests
{
    [Fact]
    public void Constructor_TrimsDetails()
    {
        var profileId = Guid.NewGuid();

        var category = new Category(profileId, "  Vintage Shirts  ", "  #111827  ", "  Daily stock  ");

        category.ProfileId.Should().Be(profileId);
        category.Name.Should().Be("Vintage Shirts");
        category.Color.Should().Be("#111827");
        category.Description.Should().Be("Daily stock");
    }

    [Fact]
    public void ReplaceTags_SoftDeletesExistingTagsAndAddsNewTags()
    {
        var category = new Category(Guid.NewGuid(), "Shirts", "#111827", null);
        var userId = Guid.NewGuid();
        var firstChange = DateTimeOffset.UtcNow.AddMinutes(-5);
        var secondChange = DateTimeOffset.UtcNow;

        category.ReplaceTags(new[] { ("#old", 1) }, userId, firstChange);

        category.ReplaceTags(new[] { ("  #new  ", 2) }, userId, secondChange);

        category.Tags.Should().HaveCount(2);
        category.Tags.Count(x => x.DeletedAt is null).Should().Be(1);
        category.Tags.Single(x => x.DeletedAt is null).TagText.Should().Be("#new");
        category.Tags.Single(x => x.DeletedAt is null).SortOrder.Should().Be(2);
        category.Tags.Single(x => x.DeletedAt is not null).DeletedBy.Should().Be(userId);
    }

    [Fact]
    public void SoftDeleteWithTags_SoftDeletesCategoryAndActiveTags()
    {
        var category = new Category(Guid.NewGuid(), "Shirts", "#111827", null);
        var userId = Guid.NewGuid();
        var deletedAt = DateTimeOffset.UtcNow;
        category.ReplaceTags(new[] { ("#shirt", 1) }, userId, deletedAt.AddMinutes(-1));

        category.SoftDeleteWithTags(userId, deletedAt);

        category.IsDeleted.Should().BeTrue();
        category.DeletedBy.Should().Be(userId);
        category.Tags.Single().DeletedAt.Should().Be(deletedAt);
    }
}