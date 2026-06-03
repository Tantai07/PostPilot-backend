using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Profiles;

public static class ProfileQueryExtensions
{
    public static IQueryable<Profile> ApplyOwnerScope(this IQueryable<Profile> query, Guid ownerUserId)
        => query.Where(x => x.OwnerUserId == ownerUserId);

    public static IQueryable<Profile> ApplyKeyword(this IQueryable<Profile> query, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return query;
        }

        keyword = keyword.Trim();

        return query.Where(x =>
            x.Name.Contains(keyword) ||
            (x.WebsiteName != null && x.WebsiteName.Contains(keyword)));
    }

    public static IQueryable<Profile> ApplyDeterministicOrder(this IQueryable<Profile> query)
        => query
            .OrderBy(x => x.Name)
            .ThenBy(x => x.WebsiteName)
            .ThenBy(x => x.Id);
}
