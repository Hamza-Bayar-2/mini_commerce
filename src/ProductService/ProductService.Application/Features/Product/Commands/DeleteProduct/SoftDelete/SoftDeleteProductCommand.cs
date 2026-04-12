using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;

public record SoftDeleteProductCommand(Guid Id) : ICommand<Result<ProductResponseDto>>;
