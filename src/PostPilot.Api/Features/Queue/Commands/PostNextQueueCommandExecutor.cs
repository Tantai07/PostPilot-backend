using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Queue.Commands;

public sealed class PostNextQueueCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public PostNextQueueCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

        var pendingItems = await _dbContext.PostQueueItems
            .Include(x => x.Post)
                .ThenInclude(x => x!.Targets)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted && x.Status == QueueItemStatus.Pending)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var nextItem = pendingItems.FirstOrDefault();
        if (nextItem is null)
        {
            return null;
        }

        nextItem.MarkPosted();
        nextItem.Post?.MarkPosted();

        for (var index = 0; index < pendingItems.Skip(1).Count(); index++)
        {
            pendingItems[index + 1].MoveTo(index + 1);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return nextItem.ToDto();
    }
}