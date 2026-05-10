using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class RefreshTokenLoggingStrategy : ILoggingStrategy<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IEventPublisherService _eventService;
    private readonly ICookieService _cookieService;

    public RefreshTokenLoggingStrategy(IEventPublisherService eventService, ICookieService cookieService)
    {
        _eventService = eventService;
        _cookieService = cookieService;
    }

    public bool CanHandle(RefreshTokenCommand request) => true;

    public async Task PublishLogAsync(RefreshTokenCommand request, Result<AuthResponseDto> response, CancellationToken ct)
    {
        var userId = _cookieService.GetUserId();
        if (userId.HasValue)
        {
            await _eventService.PublishAsync(new TokenRefreshedEvent(
                userId.Value,
                DateTime.UtcNow), ct);
        }
    }
}
