using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository _decorated;
    private readonly IDistributedCache _cache;

    public CachedProductRepository(IProductRepository decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    // magic string olmasın
    private const string AllProductsKey = "products:all";
    private static string ProductKey(Guid id) => $"products:{id}";
    private static string ProductNameKey(string name) => $"products:{name.ToLower()}";

    public async Task<Product> AddAsync(Product entity, CancellationToken ct)
    {
        await _decorated.AddAsync(entity, ct);
        await _cache.RemoveAsync(AllProductsKey, ct);
        return entity;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var json = await _cache.GetStringAsync(ProductKey(id), ct);
        if (json != null)
            return JsonSerializer.Deserialize<Product>(json); // Memory hit

        var product = await _decorated.GetByIdAsync(id, ct); // Memory miss

        if (product != null)
        {
            await _cache.SetStringAsync(
                ProductKey(id),
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                },
                ct);
        }

        return product;
    }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken ct)
    {
        // 1. Önce isimden ID'yi bulmaya çalış (Pointer)
        var idString = await _cache.GetStringAsync(ProductNameKey(name), ct);

        if (!string.IsNullOrEmpty(idString))
        {
            // 2. ID bulunduysa GetByIdAsync'e yönlendir
            if (Guid.TryParse(idString, out var id))
            {
                var cachedProduct = await GetByIdAsync(id, ct);
                if (cachedProduct != null)
                    return cachedProduct;
            }
        }

        // 3. Pointer yoksa veya asıl data cache'den silindiyse DB'den getir
        var product = await _decorated.GetByNameAsync(name, ct);

        if (product != null)
        {
            // 4. Bulunan ürünü ana anahtar (ID) ile kaydet
            await _cache.SetStringAsync(
                ProductKey(product.Id),
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                ct);

            // 5. İsimden bu ID'ye giden bir işaretçi oluştur
            await _cache.SetStringAsync(
                ProductNameKey(name),
                product.Id.ToString(),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                ct);
        }

        return product;
    }

    public async Task<Product> RemoveAsync(Product entity)
    {
        await _decorated.RemoveAsync(entity);

        await _cache.RemoveAsync(ProductKey(entity.Id));
        await _cache.RemoveAsync(ProductNameKey(entity.Name));

        return entity;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        await _decorated.UpdateAsync(entity);

        await _cache.RemoveAsync(ProductKey(entity.Id));
        await _cache.RemoveAsync(ProductNameKey(entity.Name));

        return entity;
    }
}