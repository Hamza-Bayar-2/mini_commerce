using System.Linq.Expressions;
using LogService.Application.Interfaces.Repositories;
using LogService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LogService.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly LogDbContext _db;

    public GenericRepository(LogDbContext db)
    {
        _db = db;
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct)
    {
        await _db.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Set<T>().AsNoTracking().ToListAsync(ct);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await _db.Set<T>().Where(predicate).AsNoTracking().ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Set<T>().FindAsync(new object[] { id }, ct);
    }

    public async Task<T> RemoveAsync(T entity)
    {
        _db.Set<T>().Remove(entity);
        return await Task.FromResult(entity);
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        return await Task.FromResult(entity);
    }
}
