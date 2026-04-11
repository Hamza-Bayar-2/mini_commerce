using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories;

public interface IProductStatusRepository : IGenericRepository<ProductStatus>
{
    Task<ProductStatus?> GetStatusByNamesAsync(string statusName, CancellationToken ct);
    Task<ProductStatus?> GetByIdAsync(short id, CancellationToken ct);
}