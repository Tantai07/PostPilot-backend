namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardQueueSummaryDto
{
    public int Total { get; init; }
    public int Pending { get; init; }
    public int Processing { get; init; }
    public int Posted { get; init; }
    public int Failed { get; init; }
    public int Skipped { get; init; }
    public IReadOnlyCollection<DashboardMetricDto> ByStatus { get; init; } = [];
}