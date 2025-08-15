using System.Linq.Expressions;
using PollService.API.Models;

namespace PollService.API.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<ICollection<T>> GetAllAsync();
    Task<T?> GetAsync(Guid id, Expression<Func<T, object>>? include = null);
    Task<T> CreateAndSaveAsync(T entity);
    Task UpdateAndSaveAsync(T entity);
    Task DeleteAndSaveAsync(T entity);

}
