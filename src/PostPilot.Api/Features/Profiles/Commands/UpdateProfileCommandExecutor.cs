using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Profiles.Dtos;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Profiles.Commands;

public sealed class UpdateProfileCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public UpdateProfileCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var profile = await _dbContext.Profiles.FirstOrDefaultAsync(
            x => x.Id == profileId && x.OwnerUserId == ownerUserId,
            cancellationToken);

        if (profile is null)
        {
            return null;
        }

        profile.Rename(request.Name, request.WebsiteName, request.DefaultTargets);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return profile.ToDto();
    }
}
