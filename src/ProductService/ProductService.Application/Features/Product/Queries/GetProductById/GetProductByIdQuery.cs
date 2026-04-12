using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<Result<ProductResponseDto>>;
