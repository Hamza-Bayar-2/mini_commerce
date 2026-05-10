using ProductService.Application.Features.Product.Commands.UpdateProduct;
using ProductService.Application.Common.Models;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;
using ProductService.Application.DTOs;

namespace ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class UpdateProductLoggingStrategy : ILoggingStrategy<UpdateProductCommand, Result<ProductResponseDto>>
{
    private readonly IEventPublisherService _eventPublisher;

    public UpdateProductLoggingStrategy(IEventPublisherService eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public bool CanHandle(UpdateProductCommand request) => true;

    public async Task PublishLogAsync(UpdateProductCommand request, Result<ProductResponseDto> response, CancellationToken ct)
    {
        if (response.IsSuccess && response.Data != null)
        {
            await _eventPublisher.PublishAsync(new ProductUpdatedEvent(
                response.Data.Id!.Value,
                response.Data.Name!,
                response.Data.Stock!.Value,
                response.Data.UpdatedAt!.Value), ct);
        }
    }
}
