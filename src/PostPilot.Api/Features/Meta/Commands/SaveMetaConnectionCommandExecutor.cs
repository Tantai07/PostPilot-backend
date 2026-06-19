using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Meta.Dtos;
using PostPilot.Api.Features.Meta.Security;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Meta.Commands;

public sealed class SaveMetaConnectionCommandExecutor
{
    private readonly MetaCredentialCodec _codec;
    private readonly AppDbContext _dbContext;

    public SaveMetaConnectionCommandExecutor(AppDbContext dbContext, MetaCredentialCodec codec)
    {
        _dbContext = dbContext;
        _codec = codec;
    }

    public async Task<MetaConnectionResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        MetaConnectionRequestDto request,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var facebook = await UpsertAccountAsync(profileId, SocialPlatform.Facebook, request.FacebookPageId, null, request.FacebookPageName, cancellationToken);
        await UpsertMetaTokenAsync(facebook.Id, request.PageAccessToken, request.ExpiresAt, ownerUserId, cancellationToken);

        SocialAccount? instagram = null;
        if (!string.IsNullOrWhiteSpace(request.InstagramBusinessAccountId))
        {
            instagram = await UpsertAccountAsync(
                profileId,
                SocialPlatform.Instagram,
                request.FacebookPageId,
                request.InstagramBusinessAccountId,
                string.IsNullOrWhiteSpace(request.InstagramDisplayName) ? request.FacebookPageName : request.InstagramDisplayName,
                cancellationToken);
            await UpsertMetaTokenAsync(instagram.Id, request.PageAccessToken, request.ExpiresAt, ownerUserId, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MetaConnectionResponseDto
        {
            IsConnected = true,
            FacebookPage = ToDto(facebook, request.ExpiresAt),
            InstagramBusiness = instagram is null ? null : ToDto(instagram, request.ExpiresAt)
        };
    }

    private async Task<SocialAccount> UpsertAccountAsync(Guid profileId, SocialPlatform platform, string pageId, string? igUserId, string displayName, CancellationToken cancellationToken)
    {
        var account = await _dbContext.SocialAccounts
            .Include(x => x.MetaToken)
            .FirstOrDefaultAsync(x => x.ProfileId == profileId && x.Platform == platform && !x.IsDeleted, cancellationToken);

        if (account is null)
        {
            account = new SocialAccount(profileId, platform, pageId, igUserId, displayName);
            _dbContext.SocialAccounts.Add(account);
            return account;
        }

        account.Update(pageId, igUserId, displayName);
        return account;
    }

    private async Task UpsertMetaTokenAsync(Guid socialAccountId, string value, DateTimeOffset expiresAt, Guid ownerUserId, CancellationToken cancellationToken)
    {
        var encoded = _codec.Encode(value.Trim());
        var current = await _dbContext.MetaTokens.FirstOrDefaultAsync(x => x.SocialAccountId == socialAccountId && !x.IsDeleted, cancellationToken);

        if (current is null)
        {
            _dbContext.MetaTokens.Add(new MetaToken(socialAccountId, encoded, expiresAt) { CreatedBy = ownerUserId });
            return;
        }

        current.Update(encoded, expiresAt);
    }

    private static ConnectedSocialAccountDto ToDto(SocialAccount account, DateTimeOffset expiresAt)
    {
        return new ConnectedSocialAccountDto
        {
            Id = account.Id,
            Platform = account.Platform.ToString(),
            PageId = account.PageId,
            IgUserId = account.IgUserId,
            DisplayName = account.DisplayName,
            HasCredential = true,
            ExpiresAt = expiresAt,
            IsExpired = expiresAt <= DateTimeOffset.UtcNow
        };
    }
}
