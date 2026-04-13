using LogService.Application.Interfaces.Repositories;
using LogService.Infrastructure.Persistence.Context;

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
