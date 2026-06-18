using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Api.Features.Publishing;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Posts.Commands;

public sealed class PublishPostNowCommandExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IPostPublisher _publisher;

    public PublishPostNowCommandExecutor(AppDbContext dbContext, IPostPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task<PostResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        Guid postId,
        CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .Include(x => x.Profile)
            .Include(x => x.Targets)
            .Include(x => x.Media)
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

        foreach (var target in post.Targets.Where(x => x.DeletedAt == null))
        {
            var result = await _publisher.PublishAsync(post.Id, target.TargetPlatform, cancellationToken);
            var history = new PostHistory(
                post.Id,
                target.TargetPlatform,
                result.IsSuccess ? "Posted" : "Failed",
                result.ExternalPostId,
                result.ErrorMessage)
            {
                CreatedBy = ownerUserId
            };

            _dbContext.PostHistory.Add(history);
        }

        post.MarkPosted();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return post.ToDto();
    }
}
