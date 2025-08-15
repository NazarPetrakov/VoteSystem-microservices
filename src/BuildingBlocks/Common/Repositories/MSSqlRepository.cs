using System.Linq.Expressions;
using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class MSSqlRepository<T, TContext>(TContext context) : IRepository<T>
    where T : BaseEntity
    where TContext : DbContext
{
    public async Task<ICollection<T>> GetAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }
    public async Task<T?> GetAsync(Guid id, Expression<Func<T, object>>? include = null)
    {
        return include == null
            ? await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id)
            : await context.Set<T>().Include(include).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
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

