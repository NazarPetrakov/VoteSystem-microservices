using Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions;

public static class PaginationExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageSize, int pageNumber)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
    public static PagedList<T> ToPagedList<T>(this IEnumerable<T> query, int pageSize, int pageNumber)
    {
        var count = query.Count();
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
