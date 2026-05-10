using ProductService.Application.Features.Product.Commands.DeleteProduct.HardDelete;
using ProductService.Application.Common.Models;
using ProductService.Application.Interfaces.Services;
using Shared.Events.Products;
using ProductService.Application.DTOs;

namespace ProductService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class HardDeleteProductLoggingStrategy : ILoggingStrategy<HardDeleteProductCommand, Result<ProductResponseDto>>
{
    private readonly IEventPublisherService _eventPublisher;

    public HardDeleteProductLoggingStrategy(IEventPublisherService eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public bool CanHandle(HardDeleteProductCommand request) => true;

    public async Task PublishLogAsync(HardDeleteProductCommand request, Result<ProductResponseDto> response, CancellationToken ct)
    {
        if (response.IsSuccess && response.Data != null)
        {
            await _eventPublisher.PublishAsync(new ProductHardDeletedEvent(
                response.Data.Id!.Value,
                response.Data.Name!,
                response.Data.Stock!.Value,
                DateTime.UtcNow), ct);
        }
    }
}
