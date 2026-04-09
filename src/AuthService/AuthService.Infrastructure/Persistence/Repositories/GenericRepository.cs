using AuthService.Application.Interfaces.Repositories;
using AuthService.Infrastructure.Persistence.Context;

namespace AuthService.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    private readonly AppDbContext _db;

    public GenericRepository(AppDbContext db) => _db = db;

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _db.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public Task<T> Update(T entity)
    {
        _db.Set<T>().Update(entity);
        return Task.FromResult(entity);
    }

    public async Task<T?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _db.Set<T>().FindAsync([id], cancellationToken: ct);
    }
}