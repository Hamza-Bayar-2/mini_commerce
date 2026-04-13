using AuthService.Application.Common.Models;

namespace AuthService.Application.Interfaces.Services;

public interface IEventPublisherService
{
    Task<Result<bool>> PublishAsync<T>(T @event, CancellationToken ct) where T : class;
}
