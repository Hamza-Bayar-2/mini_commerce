using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence.Context;

namespace ProductService.Infrastructure.Persistence.Repositories
{
    public class ProductStatusRepository : GenericRepository<ProductStatus>, IProductStatusRepository
    {
        private readonly AppDbContext _db;

        public ProductStatusRepository(AppDbContext db) : base(db) => _db = db;

        public async Task<ProductStatus?> GetStatusByNamesAsync(string statusName, CancellationToken ct = default)
        {
            return await _db.ProductStatuses
            .FirstOrDefaultAsync(s => statusName == s.Name, ct);
        }
    }
}