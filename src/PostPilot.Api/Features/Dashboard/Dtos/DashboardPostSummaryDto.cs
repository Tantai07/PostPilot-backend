namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardPostSummaryDto
{
    public int Total { get; init; }
    public int Draft { get; init; }
    public int Queued { get; init; }
    public int Publishing { get; init; }
    public int Posted { get; init; }
    public int Failed { get; init; }
    public int Skipped { get; init; }
    public IReadOnlyCollection<DashboardMetricDto> ByStatus { get; init; } = [];
}