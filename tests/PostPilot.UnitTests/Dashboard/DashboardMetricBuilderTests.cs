using FluentAssertions;
using PostPilot.Api.Features.Dashboard;

namespace PostPilot.UnitTests.Dashboard;

public sealed class DashboardMetricBuilderTests
{
    [Fact]
    public void FromCounts_OrdersMetricsByName()
    {
        var metrics = DashboardMetricBuilder.FromCounts([
            ("Posted", 3),
            ("Draft", 2),
            ("Failed", 1)
        ]);

        metrics.Select(x => x.Name).Should().Equal("Draft", "Failed", "Posted");
    }

    [Fact]
    public void CountByName_ReturnsMatchingCountIgnoringCase()
    {
        var metrics = DashboardMetricBuilder.FromCounts([
            ("Posted", 3),
            ("Draft", 2)
        ]);

        DashboardMetricBuilder.CountByName(metrics, "posted").Should().Be(3);
    }

    [Fact]
    public void CountByName_ReturnsZero_WhenMetricDoesNotExist()
    {
        var metrics = DashboardMetricBuilder.FromCounts([
            ("Posted", 3)
        ]);

        DashboardMetricBuilder.CountByName(metrics, "Failed").Should().Be(0);
    }
}