using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Features.Product.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<IEnumerable<ProductResponseDto>>>
{
    private readonly IProductService _productService;

    public GetAllProductsQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<IEnumerable<ProductResponseDto>>> Handle(GetAllProductsQuery request, CancellationToken ct)
    {
        return await _productService.GetAllProductsAsync(ct);
    }
}