namespace AuthService.Application.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetById(Guid id, CancellationToken ct);

    Task<T> AddAsync(T entity, CancellationToken ct);

    Task<T> Update(T entity);
}