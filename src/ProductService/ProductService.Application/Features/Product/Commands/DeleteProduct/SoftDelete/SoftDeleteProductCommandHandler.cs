using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;

public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;

    public SoftDeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<ProductResponseDto>> Handle(SoftDeleteProductCommand request, CancellationToken ct)
    {
        return await _productService.SoftDeleteProductAsync(request.Id, ct);
    }
}
