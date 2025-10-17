using System.Linq.Expressions;
using Common.Entities;

namespace Common.Repositories;

public interface IRepository<T, TId>
    where T : IEntity<TId>
    where TId : IEquatable<TId>
{
    Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes);
    Task<T> CreateAndSaveAsync(T entity);
    Task UpdateAndSaveAsync(T entity);
    Task DeleteAndSaveAsync(T entity);
    Task DeleteRangeAndSaveAsync(T[] entities);
}
