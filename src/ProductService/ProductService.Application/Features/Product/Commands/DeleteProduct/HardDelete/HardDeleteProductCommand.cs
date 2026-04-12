using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;

public record HardDeleteProductCommand(Guid Id) : ICommand<Result<ProductResponseDto>>;
