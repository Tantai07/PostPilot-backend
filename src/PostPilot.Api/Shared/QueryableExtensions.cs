using Microsoft.EntityFrameworkCore;

namespace PostPilot.Api.Shared;

public static class QueryableExtensions
{
    public static async Task<object> ToPageOrListAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        BaseQuery request,
        Func<TEntity, TDto> map,
        CancellationToken cancellationToken)
    {
        if (request.NoPaging == true || request.Page is null || request.PageSize is null)
        {
            var fullList = await query.ToListAsync(cancellationToken);
            return fullList.Select(map).ToList();
        }

        var page = Math.Max(1, request.Page.Value);
        var pageSize = Math.Clamp(request.PageSize.Value, 1, 100);
        var totalItems = await query.CountAsync(cancellationToken);
        var entities = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PageResult<TDto>
        {
            Items = entities.Select(map).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }
}
