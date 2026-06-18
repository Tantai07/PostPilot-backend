using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Dashboard.Dtos;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Dashboard.Queries;

public sealed class DashboardQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public DashboardQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var posts = _dbContext.Posts
            .AsNoTracking()
            .Where(x => x.ProfileId == profileId && !x.IsDeleted);

        var totalPosts = await posts.CountAsync(cancellationToken);
        var draftPosts = await posts.CountAsync(x => x.Status == PostStatus.Draft, cancellationToken);
        var queuedPosts = await posts.CountAsync(x => x.Status == PostStatus.Queued, cancellationToken);
        var postedPosts = await posts.CountAsync(x => x.Status == PostStatus.Posted, cancellationToken);
        var failedPosts = await posts.CountAsync(x => x.Status == PostStatus.Failed || x.Status == PostStatus.Skipped, cancellationToken);

        var pendingQueueItems = await _dbContext.PostQueueItems
            .AsNoTracking()
            .CountAsync(x => x.ProfileId == profileId && !x.IsDeleted && x.Status == QueueItemStatus.Pending, cancellationToken);

        var recentPostEntities = await _dbContext.Posts
            .AsNoTracking()
            .Include(x => x.Media)
            .Include(x => x.Targets)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted)
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new DashboardResponseDto
        {
            TotalPosts = totalPosts,
            DraftPosts = draftPosts,
            QueuedPosts = queuedPosts,
            PostedPosts = postedPosts,
            FailedPosts = failedPosts,
            QueueStatus = pendingQueueItems == 0
                ? "No posts are waiting in the manual queue."
                : $"{pendingQueueItems} post(s) waiting in the manual queue.",
            RecentPosts = recentPostEntities.Select(x => x.ToDto()).ToList(),
            Engagement = new DashboardEngagementSnapshotDto
            {
                Reach = 0,
                Impressions = 0,
                Engagement = 0
            }
        };
    }
}
