using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence.Context;

namespace ProductService.Infrastructure.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) : base(db) => _db = db;

    public async Task<Product?> GetByNameAsync(string name, CancellationToken ct)
    {
        return await _db.Products
            .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower(), ct);
    }
}