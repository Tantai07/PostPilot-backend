using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Posts.Commands;

public sealed class CreatePostDraftCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public CreatePostDraftCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PostResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        CreatePostDraftRequestDto request,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var categoryTags = await GetCategoryTagsAsync(profileId, request.CategoryId, cancellationToken);
        if (categoryTags is null)
        {
            return null;
        }

        var mediaAssets = await _dbContext.MediaAssets
            .AsNoTracking()
            .Where(x => request.MediaIds.Contains(x.Id) && x.ProfileId == profileId && !x.IsDeleted)
            .OrderBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (mediaAssets.Count != request.MediaIds.Distinct().Count())
        {
            return null;
        }

        var finalCaption = PostPreviewBuilder.BuildCaption(request.Caption, categoryTags);
        var post = new Post(profileId, request.CategoryId, finalCaption)
        {
            CreatedBy = ownerUserId
        };

        foreach (var media in mediaAssets.Select((media, index) => new { media, index }))
        {
            post.AddMedia(
                media.media.StorageProvider,
                media.media.Url,
                media.media.PublicUrl,
                media.index + 1);
        }

        foreach (var targetPlatform in PostRequestValidator.ParseTargetPlatforms(request.TargetPlatforms))
        {
            post.AddTarget(targetPlatform, Guid.Empty);
        }

        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return post.ToDto();
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
            .AnyAsync(x => x.Id == categoryId.Value && x.ProfileId == profileId && !x.IsDeleted, cancellationToken);

        if (!categoryExists)
        {
            return null;
        }

        return await _dbContext.CategoryTags
            .AsNoTracking()
            .Where(x => x.CategoryId == categoryId.Value && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => x.TagText)
            .ToListAsync(cancellationToken);
    }
}