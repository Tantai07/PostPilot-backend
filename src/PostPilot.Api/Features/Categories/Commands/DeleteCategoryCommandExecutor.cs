using Microsoft.EntityFrameworkCore;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Categories.Commands;

public sealed class DeleteCategoryCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public DeleteCategoryCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .Include(x => x.Profile)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x =>
                x.Id == categoryId
                && x.ProfileId == profileId
                && x.Profile != null
                && x.Profile.OwnerUserId == ownerUserId,
                cancellationToken);

        if (category is null)
        {
            return false;
        }

        category.SoftDeleteWithTags(ownerUserId, DateTimeOffset.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
