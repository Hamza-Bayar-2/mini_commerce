using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string? Name,
    string? Description,
    short? StatusId,
    int? Stock
) : ICommand<Result<ProductResponseDto>>;
