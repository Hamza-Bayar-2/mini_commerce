using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Queries.GetProductByName;

public record GetProductByNameQuery(string Name) : IQuery<Result<ProductResponseDto>>;
