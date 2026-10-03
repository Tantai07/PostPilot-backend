using Microsoft.EntityFrameworkCore;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Profiles.Commands;

public sealed class DeleteProfileCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public DeleteProfileCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(Guid ownerUserId, Guid profileId, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles.FirstOrDefaultAsync(
            x => x.Id == profileId && x.OwnerUserId == ownerUserId,
            cancellationToken);
        if (profile is null) return false;

        profile.SoftDelete(ownerUserId, DateTimeOffset.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
