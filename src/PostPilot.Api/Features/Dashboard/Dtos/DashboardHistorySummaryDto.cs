namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardHistorySummaryDto
{
    public int Total { get; init; }
    public int Posted { get; init; }
    public int Failed { get; init; }
    public IReadOnlyCollection<DashboardMetricDto> ByStatus { get; init; } = [];
    public IReadOnlyCollection<DashboardMetricDto> ByPlatform { get; init; } = [];
}