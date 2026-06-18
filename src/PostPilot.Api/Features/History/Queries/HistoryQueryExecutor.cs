using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.History.Dtos;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.History.Queries;

public sealed class HistoryQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public HistoryQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PostHistoryResponseDto>?> ExecuteAsync(
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

        var histories = await _dbContext.PostHistory
            .AsNoTracking()
            .Include(x => x.Post)
            .Where(x => !x.IsDeleted && x.Post != null && x.Post.ProfileId == profileId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        return histories.Select(x => x.ToDto()).ToList();
    }
}