using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Features.Product.Queries.GetProductByName;

public class GetProductByNameQueryHandler : IRequestHandler<GetProductByNameQuery, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;

    public GetProductByNameQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<ProductResponseDto>> Handle(GetProductByNameQuery request, CancellationToken ct)
    {
        return await _productService.GetProductByNameAsync(request.Name, ct);
    }
}
