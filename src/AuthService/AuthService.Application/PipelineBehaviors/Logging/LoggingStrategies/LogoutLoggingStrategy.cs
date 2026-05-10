using AuthService.Application.Common.Models;
using AuthService.Application.Features.Auth.Commands.Logout;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class LogoutLoggingStrategy : ILoggingStrategy<LogoutCommand, Result<Guid?>>
{
    private readonly IEventPublisherService _eventService;

    public LogoutLoggingStrategy(IEventPublisherService eventService)
    {
        _eventService = eventService;
    }

    public bool CanHandle(LogoutCommand request) => true;

    public async Task PublishLogAsync(LogoutCommand request, Result<Guid?> response, CancellationToken ct)
    {
        if (response.Data.HasValue)
        {
            await _eventService.PublishAsync(new UserLoggedOutEvent(
                response.Data.Value,
                DateTime.UtcNow), ct);
        }
    }
}
