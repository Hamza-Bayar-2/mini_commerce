using MassTransit;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Infrastructure.Services;

public class EventPublisherService : IEventPublisherService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventPublisherService(IPublishEndpoint publishEndpoint)
        => _publishEndpoint = publishEndpoint;

    public async Task PublishAsync<T>(T @event, CancellationToken ct) where T : class
        => await _publishEndpoint.Publish(@event, ct);
}