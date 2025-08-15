using System.Linq.Expressions;
using Common.Entities;

namespace Common.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<ICollection<T>> GetAllAsync();
    Task<T?> GetAsync(Guid id, Expression<Func<T, object>>? include = null);
    Task<T> CreateAndSaveAsync(T entity);
    Task UpdateAndSaveAsync(T entity);
    Task DeleteAndSaveAsync(T entity);
}
