using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;

namespace ProductService.Application.Features.Product.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;
    private readonly IStatusService _statusService;
    private readonly IEventPublisherService _eventService;

    public UpdateProductCommandHandler(IProductService productService, IStatusService statusService, IEventPublisherService eventService)
    {
        _productService = productService;
        _statusService = statusService;
        _eventService = eventService;
    }

    public async Task<Result<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        if (request.StatusId.HasValue && !await _statusService.IsStatusValidAsync(request.StatusId.Value, ct))
            return Result<ProductResponseDto>.Failure("Specified product status not found.");

        var productResult = await _productService.UpdateProductAsync(
            request.Id, 
            request.Name, 
            request.Description, 
            request.Stock, 
            request.StatusId, 
            ct);

        if (!productResult.IsSuccess)
            return productResult;

        await _eventService.PublishAsync(new ProductUpdatedEvent(
            productResult.Data!.Id!.Value,
            productResult.Data.Name!,
            productResult.Data.Stock!.Value,
            productResult.Data.UpdatedAt!.Value), ct);

        return productResult;
    }
}