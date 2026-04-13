using MassTransit;
using ProductService.Application.Interfaces.Services;
using ProductService.Application.Common.Models;

namespace ProductService.Infrastructure.Services;

public class EventPublisherService : IEventPublisherService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventPublisherService(IPublishEndpoint publishEndpoint)
        => _publishEndpoint = publishEndpoint;

    public async Task<Result<bool>> PublishAsync<T>(T @event, CancellationToken ct) where T : class
    {
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(2));

            await _publishEndpoint.Publish(@event, timeoutCts.Token);
            return Result<bool>.Success(true);
        }
        catch (OperationCanceledException)
        {
            return Result<bool>.Failure("Log servisi (RabbitMQ) zaman aşımına uğradı. Bağlantı kurulamıyor.");
        }
        catch (Exception)
        {
            return Result<bool>.Failure("Log servisi (RabbitMQ) şu an ayakta değil.");
        }
    }
}