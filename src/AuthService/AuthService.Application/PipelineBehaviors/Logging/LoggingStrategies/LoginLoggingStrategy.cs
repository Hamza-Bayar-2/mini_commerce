using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class LoginLoggingStrategy : ILoggingStrategy<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IEventPublisherService _eventService;

    public LoginLoggingStrategy(IEventPublisherService eventService)
    {
        _eventService = eventService;
    }

    public bool CanHandle(LoginCommand request) => true;

    public async Task PublishLogAsync(LoginCommand request, Result<AuthResponseDto> response, CancellationToken ct)
    {
        if (response.Data != null)
        {
            await _eventService.PublishAsync(new UserLoggedInEvent(
                response.Data.UserId,
                response.Data.Email,
                DateTime.UtcNow), ct);
        }
    }
}
