using PostPilot.Api.Features.Dashboard.Dtos;

namespace PostPilot.Api.Features.Dashboard;

public static class DashboardMetricBuilder
{
    public static IReadOnlyCollection<DashboardMetricDto> FromCounts(IEnumerable<(string Name, int Count)> counts)
    {
        return counts
            .Select(count => new DashboardMetricDto
            {
                Name = count.Name,
                Count = count.Count
            })
            .OrderBy(x => x.Name)
            .ToList();
    }

    public static int CountByName(IEnumerable<DashboardMetricDto> metrics, string name)
    {
        return metrics.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))?.Count ?? 0;
    }
}