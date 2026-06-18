using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Publishing;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Queue.Commands;

public sealed class PostNextQueueCommandExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IPostPublisher _publisher;

    public PostNextQueueCommandExecutor(AppDbContext dbContext, IPostPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task<QueueItemResponseDto?> ExecuteAsync(
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

        var nextItem = await _dbContext.PostQueueItems
            .Include(x => x.Post)
                .ThenInclude(x => x!.Targets)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted && x.Status == QueueItemStatus.Pending)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextItem?.Post is null)
        {
            return null;
        }

        foreach (var target in nextItem.Post.Targets.Where(x => x.DeletedAt == null))
        {
            var result = await _publisher.PublishAsync(nextItem.PostId, target.TargetPlatform, cancellationToken);
            var history = new PostHistory(
                nextItem.PostId,
                target.TargetPlatform,
                result.IsSuccess ? "Posted" : "Failed",
                result.ExternalPostId,
                result.ErrorMessage)
            {
                CreatedBy = ownerUserId
            };

            _dbContext.PostHistory.Add(history);
        }

        nextItem.MarkPosted();
        nextItem.Post.MarkPosted();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return nextItem.ToDto();
    }
}