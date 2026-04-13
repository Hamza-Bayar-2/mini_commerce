using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;

namespace ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;

public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;
    private readonly IEventPublisherService _eventService;

    public SoftDeleteProductCommandHandler(IProductService productService, IEventPublisherService eventService)
    {
        _productService = productService;
        _eventService = eventService;
    }

    public async Task<Result<ProductResponseDto>> Handle(SoftDeleteProductCommand request, CancellationToken ct)
    {
        var result = await _productService.SoftDeleteProductAsync(request.Id, ct);

        if (!result.IsSuccess)
            return result;

        var eventResult = await _eventService.PublishAsync(new ProductDeletedEvent(
            result.Data!.Id!.Value,
            result.Data.Name!,
            result.Data.Stock!.Value,
            result.Data.DeletedAt!.Value), ct);

        if (!eventResult.IsSuccess)
            return Result<ProductResponseDto>.Failure($"Ürün silindi ancak log servisi şu an ayakta olmadığı için kaydedilemedi. Hata: {eventResult.ErrorMessage}");

        return result;
    }
}
