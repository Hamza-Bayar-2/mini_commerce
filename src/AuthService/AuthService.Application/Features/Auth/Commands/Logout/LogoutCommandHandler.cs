using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using MediatR;
using Shared.Events.Auth;

namespace AuthService.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IEventPublisherService _eventService;

    public LogoutCommandHandler(
        ICookieService cookieService,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepo,
        IEventPublisherService eventService)
    {
        _cookieService = cookieService;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
        _eventService = eventService;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken ct)
    {
        Guid? userId = null;

        // 1. Get refresh token from cookie and revoke it in DB
        var refreshToken = _cookieService.GetRefreshToken();
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var validationResult = await _tokenService.ValidateRefreshTokenAsync(refreshToken, DateTime.UtcNow, ct);
            if (validationResult.IsSuccess)
            {
                var tokenEntity = validationResult.Data!;
                userId = tokenEntity.UserId;
                tokenEntity.IsRevoked = true;
                await _refreshTokenRepo.UpdateAsync(tokenEntity);
            }
        }

        // 2. Delete cookies from response
        var result = await _cookieService.DeleteCookies();

        if (!result.IsSuccess)
            return Result<Unit>.Failure(result.ErrorMessage!);

        if (userId.HasValue)
        {
            await _eventService.PublishAsync(new UserLoggedOutEvent(
                userId.Value,
                DateTime.UtcNow), ct);
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
