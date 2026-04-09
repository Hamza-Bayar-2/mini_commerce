namespace AuthService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<T> AddAsync(T entity, CancellationToken ct);

    Task<T> UpdateAsync(T entity);
}