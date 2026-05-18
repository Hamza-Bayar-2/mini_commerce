using System.Linq.Expressions;

namespace ProductService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetQueryable();

    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate, CancellationToken ct);

    Task<T> AddAsync(T entity, CancellationToken ct);

    Task<T> RemoveAsync(T entity);

    Task<T> UpdateAsync(T entity);
}