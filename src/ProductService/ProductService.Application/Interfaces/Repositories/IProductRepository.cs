using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
}