using System.Linq.Expressions;
using Common.Entities;
using Common.Pagination;

namespace Common.Repositories;

public interface IRepository<T, TId>
    where T : IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<PagedList<T>> GetAllPagedAsync(IQueryable<T> query,
        PaginationParams paginationParams);
    IQueryable<T> GetAllQuery();
    Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes);
    Task<T> CreateAndSaveAsync(T entity);
    Task UpdateAndSaveAsync(T entity);
    Task DeleteAndSaveAsync(T entity);
    Task DeleteRangeAndSaveAsync(T[] entities);
}
