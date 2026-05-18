using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;
using ProductService.Application.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Infrastructure.Services;


public class ProductManagerService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductManagerService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<Result<ProductResponseDto>> CreateProductAsync(string name, string? description, int stock, short statusId, CancellationToken ct)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Stock = stock,
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepo.AddAsync(product, ct);

        return Result<ProductResponseDto>.Success(ProductMappings.MapToDto(product));
    }

    public async Task<Result<ProductResponseDto>> UpdateProductAsync(Guid id, string? name, string? description, int? stock, short? statusId, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(id, ct);
        if (product == null)
            return Result<ProductResponseDto>.Failure("Product not found");

        if (product.DeletedAt.HasValue)
            return Result<ProductResponseDto>.Failure("Cannot update a soft-deleted product.");

        product.Name = name ?? product.Name;
        product.Description = description ?? product.Description;
        product.Stock = stock ?? product.Stock;
        product.StatusId = statusId ?? product.StatusId;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepo.UpdateAsync(product);

        return Result<ProductResponseDto>.Success(ProductMappings.MapToDto(product));
    }

    public async Task<Result<ProductResponseDto>> SoftDeleteProductAsync(Guid id, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(id, ct);
        if (product == null)
            return Result<ProductResponseDto>.Failure("Product not found");

        if (product.DeletedAt.HasValue)
            return Result<ProductResponseDto>.Failure("Product is already soft-deleted.");

        product.DeletedAt = DateTime.UtcNow;
        await _productRepo.UpdateAsync(product);

        return Result<ProductResponseDto>.Success(ProductMappings.MapToDto(product));
    }

    public async Task<Result<ProductResponseDto>> HardDeleteProductAsync(Guid id, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(id, ct);
        if (product == null)
            return Result<ProductResponseDto>.Failure("Product not found");

        var response = ProductMappings.MapToDto(product);
        await _productRepo.RemoveAsync(product);

        return Result<ProductResponseDto>.Success(response);
    }

    public async Task<Result<ProductResponseDto>> GetProductByIdAsync(Guid id, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(id, ct);

        if (product == null)
            return Result<ProductResponseDto>.Failure("Product not found");

        if (product.DeletedAt.HasValue)
            return Result<ProductResponseDto>.Failure("This product has been deleted.");

        return Result<ProductResponseDto>.Success(ProductMappings.MapToDto(product));
    }

    public async Task<Result<ProductResponseDto>> GetProductByNameAsync(string name, CancellationToken ct)
    {
        var product = await _productRepo.GetByNameAsync(name, ct);

        if (product == null)
            return Result<ProductResponseDto>.Failure("Product not found");

        if (product.DeletedAt.HasValue)
            return Result<ProductResponseDto>.Failure("This product has been deleted.");

        return Result<ProductResponseDto>.Success(ProductMappings.MapToDto(product));
    }

    public async Task<Result<IEnumerable<ProductResponseDto>>> GetAllProductsAsync(int pageNumber, int pageSize, string? search, CancellationToken ct)
    {
        // 1. ADIM: Sorgu Planının Hazırlanması (IQueryable - Deferred Execution)
        // Bu aşamada henüz veritabanına gidilmez, sadece SQL'in 'şablonu' oluşturulur.
        var query = _productRepo.GetQueryable()
            .Where(p => !p.DeletedAt.HasValue); // Sadece silinmemiş ürünleri getir.

        // 2. ADIM: Dinamik Filtreleme
        // Arama kriteri varsa, SQL planına 'LIKE' operatörü eklenir.
        if (!string.IsNullOrEmpty(search))
        {
            var lowerSearch = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerSearch));
        }

        // 3. ADIM: Sayfalama (Pagination)
        // OFFSET ve FETCH NEXT komutları SQL planına dahil edilir.
        // ToListAsync() çağrılana kadar veritabanına sorgu atılmaz.
        var products = await query
            .Skip(pageNumber * pageSize) // Belirlenen kayıt kadar atla.
            .Take(pageSize)             // Belirlenen kayıt kadar getir.
            .ToListAsync(ct);           // KRİTİK: Sorgu burada tetiklenir ve SQL veritabanına gönderilir.

        // 4. ADIM: Belleğe Alınan Verinin Dönüştürülmesi (Mapping)
        // Veritabanından gelen nesneler, API'nin döneceği DTO tipine çevrilir.
        var dtoList = products.Select(ProductMappings.MapToDto);

        return Result<IEnumerable<ProductResponseDto>>.Success(dtoList);
    }
}