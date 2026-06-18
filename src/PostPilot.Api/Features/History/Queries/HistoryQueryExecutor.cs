using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.History;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.History.Queries;

public sealed class HistoryQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public HistoryQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<object?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        HistoryQuery query,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var histories = _dbContext.PostHistory
            .AsNoTracking()
            .Include(x => x.Post)
            .ApplyProfileScope(profileId)
            .ApplyFilters(query)
            .ApplyDeterministicOrder();

        return await histories.ToPageOrListAsync(query, x => x.ToDto(), cancellationToken);
    }
}