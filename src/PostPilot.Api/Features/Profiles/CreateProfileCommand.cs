using PostPilot.Api.Features.Profiles.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Profiles;

public sealed class CreateProfileCommand
{
    private readonly AppDbContext _dbContext;

    public CreateProfileCommand(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfileResponseDto> ExecuteAsync(
        Guid ownerUserId,
        CreateProfileRequestDto request,
        CancellationToken cancellationToken)
    {
        var profile = new Profile(ownerUserId, request.Name, request.WebsiteName, request.DefaultTargets)
        {
            CreatedBy = ownerUserId
        };

        _dbContext.Profiles.Add(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return profile.ToDto();
    }
}
