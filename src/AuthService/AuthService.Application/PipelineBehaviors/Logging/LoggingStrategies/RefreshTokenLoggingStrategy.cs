using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class RefreshTokenLoggingStrategy : ILoggingStrategy<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IEventPublisherService _eventService;

    public RefreshTokenLoggingStrategy(IEventPublisherService eventService)
    {
        _eventService = eventService;
    }

    public bool CanHandle(RefreshTokenCommand request) => true;

    public async Task PublishLogAsync(RefreshTokenCommand request, Result<AuthResponseDto> response, CancellationToken ct)
    {
        if (response.Data != null)
        {
            await _eventService.PublishAsync(new TokenRefreshedEvent(
                response.Data.UserId,
                DateTime.UtcNow), ct);
        }
    }
}
