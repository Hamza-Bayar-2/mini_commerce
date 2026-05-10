using MediatR;
using ProductService.Application.Common.Models;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;

namespace ProductService.Application.Features.Product.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;
    private readonly IStatusService _statusService;
    private readonly IEventPublisherService _eventService;

    public CreateProductCommandHandler(IProductService productService, IStatusService statusService, IEventPublisherService eventService)
    {
        _productService = productService;
        _statusService = statusService;
        _eventService = eventService;
    }

    public async Task<Result<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (!await _statusService.IsStatusValidAsync(request.StatusId, ct))
            return Result<ProductResponseDto>.Failure("Specified product status not found.");

        return await _productService.CreateProductAsync(
            request.Name,
            request.Description,
            request.Stock,
            request.StatusId,
            ct);
    }
}
