using AuthService.Application.DTOs;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Interfaces.Repositories;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepo;
    private readonly ITokenService _tokenService;
    private readonly ICookieService _cookieService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepo, 
        ITokenService tokenService, 
        ICookieService cookieService)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _cookieService = cookieService;
    }
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var validationResult = await _tokenService.ValidateRefreshTokenAsync(request.TokenString, now, ct);

        if (!validationResult.IsSuccess)
            throw new Exception(validationResult.ErrorMessage);

        var user = await _userRepo.GetByIdAsync(validationResult.Data!.UserId, ct);

        if (user == null)
            throw new Exception("User not found in the data base");

        var accessResult = await _tokenService.GenerateAccessTokenAsync(user, [.. user.Roles.Select(r => r.Name)], ct);
        var refreshResult = await _tokenService.GenerateRefreshTokenAsync(user.Id, validationResult.Data.Id, now, ct);

        if (!accessResult.IsSuccess || !refreshResult.IsSuccess)
            throw new Exception("Token generation failed." 
            + accessResult.ErrorMessage + "\n" 
            + refreshResult.ErrorMessage);

        var cookieResult = await _cookieService.AppendCookies(accessResult.Data!, refreshResult.Data!.UnhashedToken);
        if (!cookieResult.IsSuccess)
            throw new Exception(cookieResult.ErrorMessage);

        return new AuthResponseDto
        {
            AccessToken = accessResult.Data!,
            RefreshToken = refreshResult.Data!.UnhashedToken,
            RefreshTokenExpiresAt = refreshResult.Data!.Entity.ExpiresAt
        };
    }
}