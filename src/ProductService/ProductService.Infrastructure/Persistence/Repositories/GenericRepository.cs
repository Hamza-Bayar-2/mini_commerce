using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Infrastructure.Persistence.Context;

namespace ProductService.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    private readonly AppDbContext _db;

    public GenericRepository(AppDbContext db) => _db = db;

    public IQueryable<T> GetQueryable() => _db.Set<T>().AsNoTracking();

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _db.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        return await Task.FromResult(entity);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Set<T>().FindAsync([id], cancellationToken: ct);
    }

    public async Task<T> RemoveAsync(T entity)
    {
        _db.Set<T>().Remove(entity);
        return await Task.FromResult(entity);
    }

    public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await _db.Set<T>().Where(predicate).ToListAsync(ct);
    }
}