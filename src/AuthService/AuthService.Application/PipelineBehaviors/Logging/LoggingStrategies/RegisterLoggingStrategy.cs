using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class RegisterLoggingStrategy : ILoggingStrategy<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IEventPublisherService _eventService;

    public RegisterLoggingStrategy(IEventPublisherService eventService)
    {
        _eventService = eventService;
    }

    public bool CanHandle(RegisterCommand request) => true;
    
    public async Task PublishLogAsync(RegisterCommand request, Result<AuthResponseDto> response, CancellationToken ct)
    {
        if (response.Data != null)
        {
            await _eventService.PublishAsync(new UserRegisteredEvent(
                response.Data.UserId,
                response.Data.Email,
                DateTime.UtcNow), ct);
        }
    }
}
