using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
{
    private readonly ICookieService _cookieService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public LogoutCommandHandler(
        ICookieService cookieService,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepo)
    {
        _cookieService = cookieService;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken ct)
    {
        // 1. Get refresh token from cookie and revoke it in DB
        var refreshToken = _cookieService.GetRefreshToken();
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var validationResult = await _tokenService.ValidateRefreshTokenAsync(refreshToken, DateTime.UtcNow, ct);
            if (validationResult.IsSuccess)
            {
                var tokenEntity = validationResult.Data!;
                tokenEntity.IsRevoked = true;
                await _refreshTokenRepo.UpdateAsync(tokenEntity);
            }
        }

        // 2. Delete cookies from response
        var result = await _cookieService.DeleteCookies();

        if (!result.IsSuccess)
            return Result<Unit>.Failure(result.ErrorMessage!);

        return Result<Unit>.Success(Unit.Value);
    }
}
