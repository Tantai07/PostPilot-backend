namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardMetricDto
{
    public string Name { get; init; } = string.Empty;
    public int Count { get; init; }
}