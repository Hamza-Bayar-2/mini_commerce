using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    short StatusId,
    int Stock
) : ICommand<Result<ProductResponseDto>>;