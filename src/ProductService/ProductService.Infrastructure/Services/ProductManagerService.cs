using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;

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

        return Result<ProductResponseDto>.Success(MapToResponse(product));
    }

    public async Task<Result<ProductResponseDto>> UpdateProductAsync(Guid id, string? name, string? description, int? stock, short? statusId, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(id, ct);
        if (product == null)
            return Result<ProductResponseDto>.Failure("Product not found");

        product.Name = name ?? product.Name;
        product.Description = description ?? product.Description;
        product.Stock = stock ?? product.Stock;
        product.StatusId = statusId ?? product.StatusId;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepo.UpdateAsync(product);

        return Result<ProductResponseDto>.Success(MapToResponse(product));
    }

    private ProductResponseDto MapToResponse(Product product)
    {
        return new ProductResponseDto
        {
            Name = product.Name,
            Description = product.Description,
            Stock = product.Stock,
            StatusName = product.Status?.Name ?? "N/A"
        };
    }
}