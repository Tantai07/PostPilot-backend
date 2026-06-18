using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Queue.Commands;

public sealed class ReorderQueueCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public ReorderQueueCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<QueueItemResponseDto>?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        ReorderQueueRequestDto request,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var queueItems = await _dbContext.PostQueueItems
            .Include(x => x.Post)
                .ThenInclude(x => x!.Targets)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted && x.Status == QueueItemStatus.Pending)
            .ToListAsync(cancellationToken);

        var requestedIds = request.QueueItemIds.Distinct().ToList();
        if (requestedIds.Count != queueItems.Count || requestedIds.Any(id => queueItems.All(x => x.Id != id)))
        {
            return null;
        }

        for (var index = 0; index < requestedIds.Count; index++)
        {
            var item = queueItems.First(x => x.Id == requestedIds[index]);
            item.MoveTo(index + 1);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return queueItems
            .OrderBy(x => x.SortOrder)
            .Select(x => x.ToDto())
            .ToList();
    }
}