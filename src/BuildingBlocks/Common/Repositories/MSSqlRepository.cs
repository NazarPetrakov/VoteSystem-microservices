using System.Linq.Expressions;
using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class MSSqlRepository<T, TContext, TId>(TContext context) : IRepository<T, TId>
    where T : BaseEntity<TId>
    where TContext : DbContext
    where TId : IEquatable<TId>
{
    public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        var query = context.Set<T>().AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync();
    }
    public async Task<T?> GetAsync(TId id, Expression<Func<T, object>>? include = null)
    {
        return include == null
            ? await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id))
            : await context.Set<T>().Include(include).AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id));
    }
    public async Task<T> CreateAndSaveAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);

        await context.SaveChangesAsync();

        return entity;
    }
    public async Task DeleteAndSaveAsync(T entity)
    {
        context.Set<T>().Remove(entity);

        await context.SaveChangesAsync();
    }
    public async Task UpdateAndSaveAsync(T entity)
    {
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }
}

