using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Media.Dtos;
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

        return new PostPreviewResponseDto
        {
            ProfileId = profileId,
            CategoryId = request.CategoryId,
            Caption = request.Caption.Trim(),
            PreviewCaption = PostPreviewBuilder.BuildCaption(request.Caption, categoryTags),
            CategoryTags = categoryTags,
            Media = mediaAssets.Select(MediaUploadResponseDto.FromEntity).ToList(),
            TargetPlatforms = PostRequestValidator.ParseTargetPlatforms(request.TargetPlatforms)
                .Select(x => x.ToString())
                .ToList()
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