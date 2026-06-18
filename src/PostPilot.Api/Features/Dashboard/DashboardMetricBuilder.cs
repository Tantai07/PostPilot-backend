using PostPilot.Api.Features.Dashboard.Dtos;

namespace PostPilot.Api.Features.Dashboard;

public static class DashboardMetricBuilder
{
    public static IReadOnlyCollection<DashboardMetricDto> ToMetrics<TKey>(IEnumerable<IGrouping<TKey, object>> groups)
        where TKey : notnull
    {
        return groups
            .Select(group => new DashboardMetricDto
            {
                Name = group.Key.ToString() ?? string.Empty,
                Count = group.Count()
            })
            .OrderBy(x => x.Name)
            .ToList();
    }

    public static int CountByName(IEnumerable<DashboardMetricDto> metrics, string name)
    {
        return metrics.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))?.Count ?? 0;
    }
}