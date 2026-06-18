using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Categories.Queries;

public static class CategoryQueryExtensions
{
    public static IQueryable<Category> ApplyProfileScope(this IQueryable<Category> query, Guid profileId)
    {
        return query.Where(x => x.ProfileId == profileId);
    }

    public static IQueryable<Category> ApplyKeyword(this IQueryable<Category> query, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        keyword = keyword.Trim();

        return query.Where(x =>
            x.Name.Contains(keyword)
            || (x.Description != null && x.Description.Contains(keyword))
            || x.Tags.Any(tag => tag.TagText.Contains(keyword)));
    }

    public static IQueryable<Category> ApplyDeterministicOrder(this IQueryable<Category> query)
    {
        return query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id);
    }
}