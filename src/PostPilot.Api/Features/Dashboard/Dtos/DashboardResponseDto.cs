namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardResponseDto
{
    public Guid ProfileId { get; init; }
    public DateTimeOffset GeneratedAt { get; init; }
    public DashboardContentSummaryDto Content { get; init; } = new();
    public DashboardPostSummaryDto Posts { get; init; } = new();
    public DashboardQueueSummaryDto Queue { get; init; } = new();
    public DashboardHistorySummaryDto History { get; init; } = new();
    public IReadOnlyCollection<DashboardRecentHistoryDto> RecentHistory { get; init; } = [];
}