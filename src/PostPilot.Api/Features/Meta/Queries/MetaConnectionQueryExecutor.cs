using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Meta.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Meta.Queries;

public sealed class MetaConnectionQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public MetaConnectionQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MetaConnectionResponseDto?> ExecuteAsync(
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

        var accounts = await _dbContext.SocialAccounts
            .AsNoTracking()
            .Include(x => x.MetaToken)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var facebook = accounts.FirstOrDefault(x => x.Platform == SocialPlatform.Facebook);
        var instagram = accounts.FirstOrDefault(x => x.Platform == SocialPlatform.Instagram);

        return new MetaConnectionResponseDto
        {
            IsConnected = facebook?.MetaToken is not null && !facebook.MetaToken.IsDeleted,
            FacebookPage = facebook is null ? null : ToDto(facebook),
            InstagramBusiness = instagram is null ? null : ToDto(instagram)
        };
    }

    private static ConnectedSocialAccountDto ToDto(SocialAccount account)
    {
        var expiresAt = account.MetaToken?.ExpiresAt;
        return new ConnectedSocialAccountDto
        {
            Id = account.Id,
            Platform = account.Platform.ToString(),
            PageId = account.PageId,
            IgUserId = account.IgUserId,
            DisplayName = account.DisplayName,
            HasCredential = account.MetaToken is not null && !account.MetaToken.IsDeleted,
            ExpiresAt = expiresAt,
            IsExpired = expiresAt is not null && expiresAt <= DateTimeOffset.UtcNow
        };
    }
}
