using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;

public class HardDeleteProductCommandHandler : IRequestHandler<HardDeleteProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;
    private readonly IEventPublisherService _eventService;

    public HardDeleteProductCommandHandler(IProductService productService, IEventPublisherService eventService)
    {
        _productService = productService;
        _eventService = eventService;
    }

    public async Task<Result<ProductResponseDto>> Handle(HardDeleteProductCommand request, CancellationToken ct)
    {
        var result = await _productService.HardDeleteProductAsync(request.Id, ct);

        if (!result.IsSuccess)
            return result;

        await _eventService.PublishAsync(new ProductHardDeletedEvent(
            result.Data!.Id!.Value,
            result.Data.Name!,
            result.Data.Stock!.Value,
            DateTime.UtcNow), ct);

        return result;
    }
}
