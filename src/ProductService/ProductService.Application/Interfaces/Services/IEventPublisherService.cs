namespace ProductService.Application.Interfaces.Services;

public interface IEventPublisherService
{
    Task PublishAsync<T>(T @event, CancellationToken ct) where T : class;
}