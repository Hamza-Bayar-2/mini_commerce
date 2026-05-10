using ProductService.Application.Features.Product.Commands.DeleteProduct.SoftDelete;
using ProductService.Application.Common.Models;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;
using ProductService.Application.DTOs;

namespace ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class SoftDeleteProductLoggingStrategy : ILoggingStrategy<SoftDeleteProductCommand, Result<ProductResponseDto>>
{
    private readonly IEventPublisherService _eventPublisher;

    public SoftDeleteProductLoggingStrategy(IEventPublisherService eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public bool CanHandle(SoftDeleteProductCommand request) => true;

    public async Task PublishLogAsync(SoftDeleteProductCommand request, Result<ProductResponseDto> response, CancellationToken ct)
    {
        if (response.IsSuccess && response.Data != null)
        {
            await _eventPublisher.PublishAsync(new ProductDeletedEvent(
                response.Data.Id!.Value,
                response.Data.Name!,
                response.Data.Stock!.Value,
                response.Data.DeletedAt!.Value), ct);
        }
    }
}
