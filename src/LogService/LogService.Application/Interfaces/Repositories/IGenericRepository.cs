using System.Linq.Expressions;

namespace LogService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
    Task<T> AddAsync(T entity, CancellationToken ct);
    Task<T> RemoveAsync(T entity);
    Task<T> UpdateAsync(T entity);
}
