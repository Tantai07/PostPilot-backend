using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Profiles;

public sealed class ProfileQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public ProfileQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<object> ExecuteAsync(Guid ownerUserId, ProfileQuery query, CancellationToken cancellationToken)
    {
        var profiles = _dbContext.Profiles
            .AsNoTracking()
            .ApplyOwnerScope(ownerUserId)
            .ApplyKeyword(query.Keyword)
            .ApplyDeterministicOrder();

        return await profiles.ToPageOrListAsync(query, x => x.ToDto(), cancellationToken);
    }
}
