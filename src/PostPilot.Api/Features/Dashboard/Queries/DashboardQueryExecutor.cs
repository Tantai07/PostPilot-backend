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

        var postStatusCounts = await _dbContext.Posts
            .AsNoTracking()
            .Where(x => x.ProfileId == profileId && !x.IsDeleted)
            .GroupBy(x => x.Status)
            .Select(x => new { Status = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        var queueStatusCounts = await _dbContext.PostQueueItems
            .AsNoTracking()
            .Where(x => x.ProfileId == profileId && !x.IsDeleted)
            .GroupBy(x => x.Status)
            .Select(x => new { Status = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        var historyStatusCounts = await _dbContext.PostHistory
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Post != null && x.Post.ProfileId == profileId)
            .GroupBy(x => x.Status)
            .Select(x => new { Status = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        var historyPlatformCounts = await _dbContext.PostHistory
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Post != null && x.Post.ProfileId == profileId)
            .GroupBy(x => x.Platform)
            .Select(x => new { Platform = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        var contentSummary = new DashboardContentSummaryDto
        {
            Categories = await _dbContext.Categories.CountAsync(x => x.ProfileId == profileId && !x.IsDeleted, cancellationToken),
            MediaAssets = await _dbContext.MediaAssets.CountAsync(x => x.ProfileId == profileId && !x.IsDeleted, cancellationToken),
            SocialAccounts = await _dbContext.SocialAccounts.CountAsync(x => x.ProfileId == profileId && !x.IsDeleted, cancellationToken)
        };

        var postMetrics = DashboardMetricBuilder.FromCounts(postStatusCounts.Select(x => (x.Status.ToString(), x.Count)));
        var queueMetrics = DashboardMetricBuilder.FromCounts(queueStatusCounts.Select(x => (x.Status.ToString(), x.Count)));
        var historyStatusMetrics = DashboardMetricBuilder.FromCounts(historyStatusCounts.Select(x => (x.Status, x.Count)));
        var historyPlatformMetrics = DashboardMetricBuilder.FromCounts(historyPlatformCounts.Select(x => (x.Platform.ToString(), x.Count)));
        var recentHistory = await GetRecentHistoryAsync(profileId, cancellationToken);

        return new DashboardResponseDto
        {
            ProfileId = profileId,
            GeneratedAt = DateTimeOffset.UtcNow,
            Content = contentSummary,
            Posts = new DashboardPostSummaryDto
            {
                Total = postStatusCounts.Sum(x => x.Count),
                Draft = DashboardMetricBuilder.CountByName(postMetrics, PostStatus.Draft.ToString()),
                Queued = DashboardMetricBuilder.CountByName(postMetrics, PostStatus.Queued.ToString()),
                Publishing = DashboardMetricBuilder.CountByName(postMetrics, PostStatus.Publishing.ToString()),
                Posted = DashboardMetricBuilder.CountByName(postMetrics, PostStatus.Posted.ToString()),
                Failed = DashboardMetricBuilder.CountByName(postMetrics, PostStatus.Failed.ToString()),
                Skipped = DashboardMetricBuilder.CountByName(postMetrics, PostStatus.Skipped.ToString()),
                ByStatus = postMetrics
            },
            Queue = new DashboardQueueSummaryDto
            {
                Total = queueStatusCounts.Sum(x => x.Count),
                Pending = DashboardMetricBuilder.CountByName(queueMetrics, QueueItemStatus.Pending.ToString()),
                Processing = DashboardMetricBuilder.CountByName(queueMetrics, QueueItemStatus.Processing.ToString()),
                Posted = DashboardMetricBuilder.CountByName(queueMetrics, QueueItemStatus.Posted.ToString()),
                Failed = DashboardMetricBuilder.CountByName(queueMetrics, QueueItemStatus.Failed.ToString()),
                Skipped = DashboardMetricBuilder.CountByName(queueMetrics, QueueItemStatus.Skipped.ToString()),
                ByStatus = queueMetrics
            },
            History = new DashboardHistorySummaryDto
            {
                Total = historyStatusCounts.Sum(x => x.Count),
                Posted = DashboardMetricBuilder.CountByName(historyStatusMetrics, "Posted"),
                Failed = DashboardMetricBuilder.CountByName(historyStatusMetrics, "Failed"),
                ByStatus = historyStatusMetrics,
                ByPlatform = historyPlatformMetrics
            },
            RecentHistory = recentHistory
        };
    }

    private async Task<IReadOnlyCollection<DashboardRecentHistoryDto>> GetRecentHistoryAsync(
        Guid profileId,
        CancellationToken cancellationToken)
    {
        var recentHistory = await _dbContext.PostHistory
            .AsNoTracking()
            .Include(x => x.Post)
            .Where(x => !x.IsDeleted && x.Post != null && x.Post.ProfileId == profileId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(5)
            .ToListAsync(cancellationToken);

        return recentHistory
            .Select(x => new DashboardRecentHistoryDto
            {
                HistoryId = x.Id,
                PostId = x.PostId,
                Caption = x.Post?.Caption ?? string.Empty,
                Platform = x.Platform.ToString(),
                Status = x.Status,
                ExternalPostId = x.ExternalPostId,
                ErrorMessage = x.ErrorMessage,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}