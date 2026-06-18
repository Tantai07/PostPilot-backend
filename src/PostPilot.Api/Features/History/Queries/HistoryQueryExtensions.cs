using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;

namespace PostPilot.Api.Features.History.Queries;

public static class HistoryQueryExtensions
{
    public static IQueryable<PostHistory> ApplyProfileScope(this IQueryable<PostHistory> query, Guid profileId)
    {
        return query.Where(x => x.Post != null && x.Post.ProfileId == profileId);
    }

    public static IQueryable<PostHistory> ApplyFilters(this IQueryable<PostHistory> query, HistoryQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Platform)
            && Enum.TryParse<PostTargetPlatform>(NormalizeEnumInput(request.Platform), ignoreCase: true, out var platform))
        {
            query = query.Where(x => x.Platform == platform);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status.Trim();
            query = query.Where(x => x.Status == status);
        }

        if (request.CategoryId is not null)
        {
            query = query.Where(x => x.Post != null && x.Post.CategoryId == request.CategoryId.Value);
        }

        if (request.From is not null)
        {
            query = query.Where(x => x.CreatedAt >= request.From.Value);
        }

        if (request.To is not null)
        {
            query = query.Where(x => x.CreatedAt <= request.To.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x =>
                (x.Post != null && x.Post.Caption.Contains(keyword))
                || (x.ExternalPostId != null && x.ExternalPostId.Contains(keyword))
                || (x.ErrorMessage != null && x.ErrorMessage.Contains(keyword)));
        }

        return query;
    }

    public static IQueryable<PostHistory> ApplyDeterministicOrder(this IQueryable<PostHistory> query)
    {
        return query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id);
    }

    private static string NormalizeEnumInput(string value)
    {
        return value.Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).Trim();
    }
}