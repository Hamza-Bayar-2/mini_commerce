using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository _decorated;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedProductRepository> _logger;

    private const string AllProductsKey = "products:all";
    private static string ProductKey(Guid id) => $"products:{id}";
    private static string ProductNameKey(string name) => $"products:name:{name.ToLower()}";

    public CachedProductRepository(
        IProductRepository decorated,
        IDistributedCache cache,
        ILogger<CachedProductRepository> logger)
    {
        _decorated = decorated;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var json = await _cache.GetStringAsync(AllProductsKey, ct);
            if (json is not null)
                return JsonSerializer.Deserialize<IEnumerable<Product>>(json) ?? Array.Empty<Product>();

            var products = await _decorated.GetAllAsync(ct);
            if (products is not null && products.Any())
            {
                await _cache.SetStringAsync(
                    AllProductsKey,
                    JsonSerializer.Serialize(products),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    }, ct);
            }

            return products ?? Array.Empty<Product>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache unavailable, falling back to database.");
            return await _decorated.GetAllAsync(ct);
        }
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var json = await _cache.GetStringAsync(ProductKey(id), ct);
            if (json is not null)
                return JsonSerializer.Deserialize<Product>(json);

            var product = await _decorated.GetByIdAsync(id, ct);
            if (product is not null)
                await _cache.SetStringAsync(
                    ProductKey(id),
                    JsonSerializer.Serialize(product),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    }, ct);

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache unavailable, falling back to database.");
            return await _decorated.GetByIdAsync(id, ct);
        }
    }

    public async Task<Product?> GetByNameAsync(string name, CancellationToken ct)
    {
        try
        {
            var idString = await _cache.GetStringAsync(ProductNameKey(name), ct);
            if (!string.IsNullOrEmpty(idString) && Guid.TryParse(idString, out var id))
            {
                var json = await _cache.GetStringAsync(ProductKey(id), ct);
                if (json is not null)
                    return JsonSerializer.Deserialize<Product>(json);
            }

            var product = await _decorated.GetByNameAsync(name, ct);
            if (product is not null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                var pointerOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4)
                };
                await _cache.SetStringAsync(ProductKey(product.Id),
                    JsonSerializer.Serialize(product), options, ct);
                await _cache.SetStringAsync(ProductNameKey(name),
                    product.Id.ToString(), pointerOptions, ct);
            }

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache unavailable, falling back to database.");
            return await _decorated.GetByNameAsync(name, ct);
        }
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken ct)
    {
        await _decorated.AddAsync(entity, ct);

        try
        {
            await _cache.RemoveAsync(AllProductsKey, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache invalidation failed after add.");
        }

        return entity;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        await _decorated.UpdateAsync(entity);

        try
        {
            await _cache.RemoveAsync(ProductKey(entity.Id));
            await _cache.RemoveAsync(ProductNameKey(entity.Name));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache invalidation failed after update.");
        }

        return entity;
    }

    public async Task<Product> RemoveAsync(Product entity)
    {
        await _decorated.RemoveAsync(entity);

        try
        {
            await _cache.RemoveAsync(ProductKey(entity.Id));
            await _cache.RemoveAsync(ProductNameKey(entity.Name));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache invalidation failed after remove.");
        }

        return entity;
    }
}