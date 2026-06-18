using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Categories;
using PostPilot.Api.Shared;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Categories.Queries;

public sealed class CategoryQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public CategoryQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<object?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        CategoryQuery query,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var categories = _dbContext.Categories
            .AsNoTracking()
            .Include(x => x.Tags)
            .ApplyProfileScope(profileId)
            .ApplyKeyword(query.Keyword)
            .ApplyDeterministicOrder();

        return await categories.ToPageOrListAsync(query, x => x.ToDto(), cancellationToken);
    }
}