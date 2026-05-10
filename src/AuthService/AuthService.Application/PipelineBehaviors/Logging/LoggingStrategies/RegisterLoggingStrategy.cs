using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class RegisterLoggingStrategy : ILoggingStrategy<RegisterCommand, Result<RegisterResponseDto>>
{
    private readonly IEventPublisherService _eventService;

    public RegisterLoggingStrategy(IEventPublisherService eventService)
    {
        _eventService = eventService;
    }

    public bool CanHandle(RegisterCommand request) => true;
    
    public async Task PublishLogAsync(RegisterCommand request, Result<RegisterResponseDto> response, CancellationToken ct)
    {
        if (response.Data != null)
        {
            await _eventService.PublishAsync(new UserRegisteredEvent(
                response.Data.UserId,
                response.Data.Email,
                response.Data.FullName,
                DateTime.UtcNow), ct);
        }
    }
}
