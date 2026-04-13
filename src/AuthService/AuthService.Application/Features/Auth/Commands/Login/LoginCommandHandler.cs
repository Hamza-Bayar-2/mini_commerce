using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using MediatR;
using Shared.Events.Auth;

namespace AuthService.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IEventPublisherService _eventService;

    public LoginCommandHandler(
        IUserRepository _userRepo,
        ITokenService _tokenService,
        ICookieService _cookieService,
        IEventPublisherService eventService)
    {
        this._userRepo = _userRepo;
        this._tokenService = _tokenService;
        this._cookieService = _cookieService;
        _eventService = eventService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email, ct);

        if (user == null || user.UserCredential == null)
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.UserCredential.PasswordHash);

        if (!isPasswordValid)
            return Result<AuthResponseDto>.Failure("Invalid email or password.");

        var now = DateTime.UtcNow;

        // Generate tokens
        var roles = user.Roles.Select(r => r.Name).ToList();
        var accessTokenResult = await _tokenService.GenerateAccessTokenAsync(user, roles, ct);
        var refreshTokenResult = await _tokenService.GenerateRefreshTokenAsync(user.Id, null, now, ct);

        if (!accessTokenResult.IsSuccess || !refreshTokenResult.IsSuccess)
            return Result<AuthResponseDto>.Failure("Token generation failed.");

        // Append to cookies
        var cookieResult = await _cookieService.AppendCookies(accessTokenResult.Data!, refreshTokenResult.Data!.UnhashedToken);
        if (!cookieResult.IsSuccess)
            return Result<AuthResponseDto>.Failure(cookieResult.ErrorMessage!);

        var eventResult = await _eventService.PublishAsync(new UserLoggedInEvent(
            user.Id,
            user.Email,
            now), ct);

        if (!eventResult.IsSuccess)
            return Result<AuthResponseDto>.Failure($"Giriş yapıldı ancak log servisi şu an ayakta olmadığı için kaydedilemedi. Hata: {eventResult.ErrorMessage}");
        
        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            AccessToken = accessTokenResult.Data!,
            RefreshToken = refreshTokenResult.Data!.UnhashedToken,
            RefreshTokenExpiresAt = refreshTokenResult.Data!.Entity.ExpiresAt
        });
    }
}
