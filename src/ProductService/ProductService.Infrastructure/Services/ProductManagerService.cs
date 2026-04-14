using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;
using ProductService.Application.Mappings;

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

    public async Task<Result<IEnumerable<ProductResponseDto>>> GetAllProductsAsync(CancellationToken ct)
    {
        var products = await _productRepo.GetAllAsync(ct);
        
        var activeProducts = products.Where(p => !p.DeletedAt.HasValue);
        var dtoList = activeProducts.Select(ProductMappings.MapToDto);
        
        return Result<IEnumerable<ProductResponseDto>>.Success(dtoList);
    }
}