using ProductService.Application.Common.Models;

namespace ProductService.Application.Interfaces.Services;

public interface IEventPublisherService
{
    Task<Result<bool>> PublishAsync<T>(T @event, CancellationToken ct) where T : class;
}