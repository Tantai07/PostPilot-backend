namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardRecentHistoryDto
{
    public Guid HistoryId { get; init; }
    public Guid PostId { get; init; }
    public string Caption { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? ExternalPostId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}