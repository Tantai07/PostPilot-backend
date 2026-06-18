using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
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

        if (request.CategoryId is not null)
        {
            var categoryExists = await _dbContext.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.CategoryId && x.ProfileId == profileId && !x.IsDeleted, cancellationToken);

            if (!categoryExists)
            {
                return null;
            }
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

        var post = new Post(profileId, request.CategoryId, request.Caption)
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

        foreach (var targetPlatform in ParseTargetPlatforms(request.TargetPlatforms))
        {
            post.AddTarget(targetPlatform, Guid.Empty);
        }

        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return post.ToDto();
    }

    private static IEnumerable<PostTargetPlatform> ParseTargetPlatforms(IEnumerable<string> targetPlatforms)
    {
        return targetPlatforms
            .Select(ParseTargetPlatform)
            .Where(x => x is not null)
            .Select(x => x!.Value)
            .Distinct();
    }

    private static PostTargetPlatform? ParseTargetPlatform(string targetPlatform)
    {
        return targetPlatform.Trim().ToLowerInvariant() switch
        {
            "facebook page" or "facebookpage" => PostTargetPlatform.FacebookPage,
            "instagram feed" or "instagramfeed" => PostTargetPlatform.InstagramFeed,
            "instagram story" or "instagramstory" => PostTargetPlatform.InstagramStory,
            _ => null
        };
    }
}