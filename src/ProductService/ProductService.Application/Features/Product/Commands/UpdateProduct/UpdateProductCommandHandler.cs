using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;
    private readonly IStatusService _statusService;

    public UpdateProductCommandHandler(IProductService productService, IStatusService statusService)
    {
        _productService = productService;
        _statusService = statusService;
    }

    public async Task<Result<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        if (request.StatusId.HasValue && !await _statusService.IsStatusValidAsync(request.StatusId.Value, ct))
            return Result<ProductResponseDto>.Failure("Specified product status not found.");

        return await _productService.UpdateProductAsync(
            request.Id, 
            request.Name, 
            request.Description, 
            request.Stock, 
            request.StatusId, 
            ct);
    }
}