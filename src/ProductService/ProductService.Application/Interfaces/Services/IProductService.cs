using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;

namespace ProductService.Application.Interfaces.Services;

public interface IProductService
{
    Task<Result<ProductResponseDto>> CreateProductAsync(string name, string? description, int stock, short statusId, CancellationToken ct);
    Task<Result<ProductResponseDto>> UpdateProductAsync(Guid id, string? name, string? description, int? stock, short? statusId, CancellationToken ct);
    Task<Result<ProductResponseDto>> SoftDeleteProductAsync(Guid id, CancellationToken ct);
    Task<Result<ProductResponseDto>> HardDeleteProductAsync(Guid id, CancellationToken ct);
    Task<Result<ProductResponseDto>> GetProductByIdAsync(Guid id, CancellationToken ct);
    Task<Result<ProductResponseDto>> GetProductByNameAsync(string name, CancellationToken ct);
    Task<Result<IEnumerable<ProductResponseDto>>> GetAllProductsAsync(CancellationToken ct);
}

