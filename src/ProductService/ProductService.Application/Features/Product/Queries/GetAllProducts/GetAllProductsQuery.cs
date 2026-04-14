using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Product.Queries.GetAllProducts;

public record GetAllProductsQuery() : IQuery<Result<IEnumerable<ProductResponseDto>>>;