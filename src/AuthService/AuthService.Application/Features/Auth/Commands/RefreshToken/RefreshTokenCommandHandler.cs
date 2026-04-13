using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Interfaces.Repositories;
using MediatR;
using Shared.Events.Auth;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;
    private readonly IEventPublisherService _eventService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepo,
        ITokenService tokenService,
        ICookieService cookieService,
        IEventPublisherService eventService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _cookieService = cookieService;
        _eventService = eventService;
    }
    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var refreshToken = _cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
            return Result<AuthResponseDto>.Failure("Refresh token not found in cookies.");

        var now = DateTime.UtcNow;

        var validationResult = await _tokenService.ValidateRefreshTokenAsync(refreshToken, now, ct);

        if (!validationResult.IsSuccess)
            return Result<AuthResponseDto>.Failure(validationResult.ErrorMessage!);

        var user = await _userRepo.GetByIdAsync(validationResult.Data!.UserId, ct);

        if (user == null)
            return Result<AuthResponseDto>.Failure("User not found in the data base");

        var accessResult = await _tokenService.GenerateAccessTokenAsync(user, [.. user.Roles.Select(r => r.Name)], ct);
        var refreshResult = await _tokenService.GenerateRefreshTokenAsync(user.Id, validationResult.Data.Id, now, ct);

        if (!accessResult.IsSuccess || !refreshResult.IsSuccess)
            return Result<AuthResponseDto>.Failure("Token generation failed."
            + accessResult.ErrorMessage + "\n"
            + refreshResult.ErrorMessage);

        var cookieResult = await _cookieService.AppendCookies(accessResult.Data!, refreshResult.Data!.UnhashedToken);
        if (!cookieResult.IsSuccess)
            return Result<AuthResponseDto>.Failure(cookieResult.ErrorMessage!);

        await _eventService.PublishAsync(new TokenRefreshedEvent(
            user.Id, 
            now), ct);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            AccessToken = accessResult.Data!,
            RefreshToken = refreshResult.Data!.UnhashedToken,
            RefreshTokenExpiresAt = refreshResult.Data!.Entity.ExpiresAt
        });
    }
}