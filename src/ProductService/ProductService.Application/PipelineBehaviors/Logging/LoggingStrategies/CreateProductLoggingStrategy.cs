using ProductService.Application.Features.Product.Commands.CreateProduct;
using ProductService.Application.Common.Models;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;
using ProductService.Application.DTOs;

namespace ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class CreateProductLoggingStrategy : ILoggingStrategy<CreateProductCommand, Result<ProductResponseDto>>
{
    private readonly IEventPublisherService _eventPublisher;

    public CreateProductLoggingStrategy(IEventPublisherService eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public bool CanHandle(CreateProductCommand request) => true;

    public async Task PublishLogAsync(CreateProductCommand request, Result<ProductResponseDto> response, CancellationToken ct)
    {
        if (response.IsSuccess && response.Data != null)
        {
            await _eventPublisher.PublishAsync(new ProductCreatedEvent(
                response.Data.Id!.Value,
                response.Data.Name!,
                response.Data.Stock!.Value,
                response.Data.CreatedAt!.Value), ct);
        }
    }
}
