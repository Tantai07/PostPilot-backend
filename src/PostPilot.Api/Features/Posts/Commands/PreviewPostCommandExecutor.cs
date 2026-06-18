using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Posts.Commands;

public sealed class PreviewPostCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public PreviewPostCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PostPreviewResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        PreviewPostRequestDto request,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var categoryTags = await GetCategoryTagsAsync(profileId, request.CategoryId, cancellationToken);
        if (categoryTags is null)
        {
            return null;
        }

        var targetsExist = await TargetsBelongToProfileAsync(profileId, request.Targets, cancellationToken);
        if (!targetsExist)
        {
            return null;
        }

        return new PostPreviewResponseDto
        {
            ProfileId = profileId,
            CategoryId = request.CategoryId,
            Caption = request.Caption.Trim(),
            PreviewCaption = PostPreviewBuilder.BuildCaption(request.Caption, categoryTags),
            CategoryTags = categoryTags,
            Media = request.Media,
            Targets = request.Targets
        };
    }

    private async Task<IReadOnlyList<string>?> GetCategoryTagsAsync(
        Guid profileId,
        Guid? categoryId,
        CancellationToken cancellationToken)
    {
        if (categoryId is null)
        {
            return [];
        }

        var categoryExists = await _dbContext.Categories
            .AsNoTracking()
            .AnyAsync(x => x.Id == categoryId.Value && x.ProfileId == profileId, cancellationToken);

        if (!categoryExists)
        {
            return null;
        }

        return await _dbContext.CategoryTags
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId.Value)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => x.TagText)
            .ToListAsync(cancellationToken);
    }

    private async Task<bool> TargetsBelongToProfileAsync(
        Guid profileId,
        IEnumerable<PostTargetRequestDto> targets,
        CancellationToken cancellationToken)
    {
        var targetAccountIds = targets
            .Select(x => x.TargetAccountId)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (targetAccountIds.Count == 0)
        {
            return true;
        }

        var matchingCount = await _dbContext.SocialAccounts
            .AsNoTracking()
            .CountAsync(
                x => x.ProfileId == profileId && targetAccountIds.Contains(x.Id),
                cancellationToken);

        return matchingCount == targetAccountIds.Count;
    }
}