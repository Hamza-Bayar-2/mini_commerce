using AuthService.Application.Common.Models;
using AuthService.Application.Features.Auth.Commands.Logout;
using AuthService.Application.Interfaces.Services;
using MediatR;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class LogoutLoggingStrategy : ILoggingStrategy<LogoutCommand, Result<MediatR.Unit>>
{
    private readonly IEventPublisherService _eventService;
    private readonly ICookieService _cookieService;

    public LogoutLoggingStrategy(IEventPublisherService eventService, ICookieService cookieService)
    {
        _eventService = eventService;
        _cookieService = cookieService;
    }

    public bool CanHandle(LogoutCommand request) => true;

    public async Task PublishLogAsync(LogoutCommand request, Result<MediatR.Unit> response, CancellationToken ct)
    {
        var userId = _cookieService.GetUserId();
        if (userId.HasValue)
        {
            await _eventService.PublishAsync(new UserLoggedOutEvent(
                userId.Value,
                DateTime.UtcNow), ct);
        }
    }
}
