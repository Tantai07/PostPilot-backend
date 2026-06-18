using PostPilot.Api.Features.Posts.Dtos;

namespace PostPilot.Api.Features.Dashboard.Dtos;

public sealed class DashboardResponseDto
{
    public int TotalPosts { get; init; }
    public int DraftPosts { get; init; }
    public int QueuedPosts { get; init; }
    public int PostedPosts { get; init; }
    public int FailedPosts { get; init; }
    public string QueueStatus { get; init; } = string.Empty;
    public IReadOnlyCollection<PostResponseDto> RecentPosts { get; init; } = [];
    public DashboardEngagementSnapshotDto Engagement { get; init; } = new();
}

public sealed class DashboardEngagementSnapshotDto
{
    public int Reach { get; init; }
    public int Impressions { get; init; }
    public int Engagement { get; init; }
}
