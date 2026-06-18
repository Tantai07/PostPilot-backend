using FluentAssertions;
using PostPilot.Api.Features.History.Queries;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.UnitTests.History;

public sealed class HistoryQueryExtensionTests
{
    [Fact]
    public void ApplyFilters_FiltersByStatus()
    {
        var histories = new[]
        {
            new PostHistory(Guid.NewGuid(), PostTargetPlatform.FacebookPage, "Posted", "mock-1"),
            new PostHistory(Guid.NewGuid(), PostTargetPlatform.FacebookPage, "Failed", null, "error")
        }.AsQueryable();

        var result = histories.ApplyFilters(new HistoryQuery { Status = "Posted" }).ToList();

        result.Should().ContainSingle();
        result[0].Status.Should().Be("Posted");
    }

    [Fact]
    public void ApplyFilters_FiltersByPlatformWithFlexibleInput()
    {
        var histories = new[]
        {
            new PostHistory(Guid.NewGuid(), PostTargetPlatform.FacebookPage, "Posted", "mock-1"),
            new PostHistory(Guid.NewGuid(), PostTargetPlatform.InstagramFeed, "Posted", "mock-2")
        }.AsQueryable();

        var result = histories.ApplyFilters(new HistoryQuery { Platform = "Instagram Feed" }).ToList();

        result.Should().ContainSingle();
        result[0].Platform.Should().Be(PostTargetPlatform.InstagramFeed);
    }

    [Fact]
    public void ApplyDeterministicOrder_ReturnsNewestFirst()
    {
        var oldHistory = new PostHistory(Guid.NewGuid(), PostTargetPlatform.FacebookPage, "Posted", "old")
        {
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        var newHistory = new PostHistory(Guid.NewGuid(), PostTargetPlatform.FacebookPage, "Posted", "new")
        {
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = new[] { oldHistory, newHistory }.AsQueryable().ApplyDeterministicOrder().ToList();

        result[0].ExternalPostId.Should().Be("new");
        result[1].ExternalPostId.Should().Be("old");
    }
}