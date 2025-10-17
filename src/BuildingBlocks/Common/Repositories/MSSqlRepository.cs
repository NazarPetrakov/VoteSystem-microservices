using System.Linq.Expressions;
using Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class MSSqlRepository<T, TContext, TId> : IRepository<T, TId>
    where T : class, IEntity<TId>
    where TContext : DbContext
    where TId : IEquatable<TId>
{
    private readonly TContext _context;
    private readonly DbSet<T> _dbSet;

    public MSSqlRepository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        var query = _dbSet.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync();
    }
    public async Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id));
    }
    public async Task<T> CreateAndSaveAsync(T entity)
    {
        await _dbSet.AddAsync(entity);

        await _context.SaveChangesAsync();

        return entity;
    }
    public async Task DeleteAndSaveAsync(T entity)
    {
        _dbSet.Remove(entity);

        await _context.SaveChangesAsync();
    }
    public async Task UpdateAndSaveAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }
    public async Task DeleteRangeAndSaveAsync(T[] entities)
    {
        _dbSet.RemoveRange(entities);

        await _context.SaveChangesAsync();
    }
}

