using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Queue.Queries;

public sealed class QueueQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public QueueQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<QueueItemResponseDto>?> ExecuteAsync(
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

        var queueItems = await _dbContext.PostQueueItems
            .AsNoTracking()
            .Include(x => x.Post)
                .ThenInclude(x => x!.Targets)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted && x.Status == QueueItemStatus.Pending)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return queueItems.Select(x => x.ToDto()).ToList();
    }
}