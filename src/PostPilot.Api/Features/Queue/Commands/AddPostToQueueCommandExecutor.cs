using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Queue.Commands;

public sealed class AddPostToQueueCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public AddPostToQueueCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QueueItemResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        Guid postId,
        DateTimeOffset? scheduledAt,
        CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .Include(x => x.Profile)
            .Include(x => x.Targets)
            .FirstOrDefaultAsync(x =>
                x.Id == postId
                && x.ProfileId == profileId
                && !x.IsDeleted
                && x.Profile != null
                && x.Profile.OwnerUserId == ownerUserId,
                cancellationToken);

        if (post is null)
        {
            return null;
        }

        var existingQueueItem = await _dbContext.PostQueueItems
            .Include(x => x.Post)
                .ThenInclude(x => x!.Targets)
            .FirstOrDefaultAsync(x =>
                x.ProfileId == profileId
                && x.PostId == postId
                && !x.IsDeleted
                && x.Status == QueueItemStatus.Pending,
                cancellationToken);

        if (existingQueueItem is not null)
        {
            return existingQueueItem.ToDto();
        }

        var nextSortOrder = await _dbContext.PostQueueItems
            .Where(x => x.ProfileId == profileId && !x.IsDeleted && x.Status == QueueItemStatus.Pending)
            .Select(x => (int?)x.SortOrder)
            .MaxAsync(cancellationToken) ?? 0;

        post.MarkQueued();
        var queueItem = new PostQueueItem(profileId, postId, nextSortOrder + 1, scheduledAt)
        {
            CreatedBy = ownerUserId
        };

        _dbContext.PostQueueItems.Add(queueItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return queueItem.ToDto();
    }
}