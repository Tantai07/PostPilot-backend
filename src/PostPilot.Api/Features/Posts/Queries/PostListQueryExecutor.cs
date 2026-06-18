using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Posts.Queries;

public sealed class PostListQueryExecutor
{
    private readonly AppDbContext _dbContext;

    public PostListQueryExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<PostResponseDto>?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        PostListQuery query,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var posts = _dbContext.Posts
            .AsNoTracking()
            .Include(x => x.Media)
            .Include(x => x.Targets)
            .Where(x => x.ProfileId == profileId && !x.IsDeleted);

        if (TryParseStatus(query.Status, out var status))
        {
            posts = posts.Where(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            posts = posts.Where(x => x.Caption.Contains(keyword));
        }

        return await posts
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Take(query.PageSize ?? 50)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);
    }

    private static bool TryParseStatus(string? value, out PostStatus status)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            status = default;
            return false;
        }

        return Enum.TryParse(value, true, out status);
    }
}
