using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Publishing.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Publishing.Commands;

public sealed class PublishPostCommandExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IPostPublisher _publisher;

    public PublishPostCommandExecutor(AppDbContext dbContext, IPostPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task<PublishPostResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        Guid postId,
        CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .Include(x => x.Profile)
            .Include(x => x.Targets)
            .FirstOrDefaultAsync(x =>
                x.Id == postId
                && x.ProfileId == profileId
                && !x.IsDeleted
                && x.Profile != null
                && x.Profile.OwnerUserId == ownerUserId,
                cancellationToken);

        if (post is null)
        {
            return null;
        }

        var targets = post.Targets
            .Where(x => x.DeletedAt is null)
            .OrderBy(x => x.TargetPlatform)
            .ToList();

        if (targets.Count == 0)
        {
            return new PublishPostResponseDto
            {
                PostId = post.Id,
                ProfileId = post.ProfileId,
                Status = post.Status.ToString(),
                IsSuccess = false,
                PublishedAt = DateTimeOffset.UtcNow,
                Targets = []
            };
        }

        post.MarkPublishing();
        var targetResults = new List<PublishTargetResultDto>();

        foreach (var target in targets)
        {
            var result = await _publisher.PublishAsync(post.Id, target.TargetPlatform, cancellationToken);
            var status = result.IsSuccess ? "Posted" : "Failed";

            _dbContext.PostHistory.Add(new PostHistory(
                post.Id,
                target.TargetPlatform,
                status,
                result.ExternalPostId,
                result.ErrorMessage)
            {
                CreatedBy = ownerUserId
            });

            targetResults.Add(new PublishTargetResultDto
            {
                Platform = target.TargetPlatform.ToString(),
                IsSuccess = result.IsSuccess,
                ExternalPostId = result.ExternalPostId,
                ErrorMessage = result.ErrorMessage
            });
        }

        if (targetResults.All(x => x.IsSuccess))
        {
            post.MarkPosted();
        }
        else
        {
            post.MarkFailed();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PublishPostResponseDto
        {
            PostId = post.Id,
            ProfileId = post.ProfileId,
            Status = post.Status.ToString(),
            IsSuccess = targetResults.All(x => x.IsSuccess),
            PublishedAt = DateTimeOffset.UtcNow,
            Targets = targetResults
        };
    }
}