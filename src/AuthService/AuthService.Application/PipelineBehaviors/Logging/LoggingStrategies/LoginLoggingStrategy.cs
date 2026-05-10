using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using Shared.Events.Auth;

namespace AuthService.Application.PipelineBehaviors.Logging.LoggingStrategies;

public class LoginLoggingStrategy : ILoggingStrategy<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IEventPublisherService _eventService;
    private readonly IUserRepository _userRepo;

    public LoginLoggingStrategy(IEventPublisherService eventService, IUserRepository userRepo)
    {
        _eventService = eventService;
        _userRepo = userRepo;
    }

    public bool CanHandle(LoginCommand request) => true;

    public async Task PublishLogAsync(LoginCommand request, Result<AuthResponseDto> response, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, ct);
        if (user != null)
        {
            await _eventService.PublishAsync(new UserLoggedInEvent(
                user.Id,
                user.Email,
                DateTime.UtcNow), ct);
        }
    }
}
