using System.Linq.Expressions;
using Common.Entities;
using Common.Pagination;
using Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class MongoRepository<T, TContext, TId> : IRepository<T, TId>
    where T : class, IEntity<TId>
    where TContext : DbContext
    where TId : IEquatable<TId>
{
    private readonly TContext _context;
    private readonly DbSet<T> _dbSet;

    public MongoRepository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public async Task<PagedList<T>> GetAllPagedAsync(IQueryable<T> query,
        PaginationParams paginationParams)
    {
        var pagedList = await query.ToPagedListAsync(paginationParams.PageSize, paginationParams.PageNumber);

        return pagedList;
    }
    public IQueryable<T> GetAllQuery()
    {
        var query = _dbSet.AsQueryable();

        return query;
    }
    public async Task<T?> GetAsync(TId id, params Expression<Func<T, object>>[] includes)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Id.Equals(id));
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
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteRangeAndSaveAsync(T[] entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
}
