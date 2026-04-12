using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;

public class HardDeleteProductCommandHandler : IRequestHandler<HardDeleteProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;

    public HardDeleteProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<ProductResponseDto>> Handle(HardDeleteProductCommand request, CancellationToken ct)
    {
        return await _productService.HardDeleteProductAsync(request.Id, ct);
    }
}
