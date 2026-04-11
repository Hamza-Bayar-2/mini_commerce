using AuthService.Application.DTOs;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using MediatR;
using AuthService.Application.Common.Models;
using AuthService.Domain.Enums;

namespace AuthService.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
  private readonly IUserRepository _userRepo;
  private readonly IRoleRepository _roleRepo;
  private readonly ITokenService _tokenService;
  private readonly ICookieService _cookieService;

  public RegisterCommandHandler(
    IUserRepository userRepo,
    IRoleRepository roleRepo,
    ITokenService tokenService,
    ICookieService cookieService)
  {
    _userRepo = userRepo;
    _roleRepo = roleRepo;
    _tokenService = tokenService;
    _cookieService = cookieService;
  }

  public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
  {
    var existing = await _userRepo.GetByEmailAsync(request.Email, ct);
    if (existing is not null)
      return Result<AuthResponseDto>.Failure("Email already exists.");

    var now = DateTime.UtcNow;

    var roles = await _roleRepo.GetRolesByIdsAsync([(short)Roles.CUSTOMER], ct);

    var user = new User
    {
      Id = Guid.NewGuid(),
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      PhoneNumber = request.PhoneNumber,
      CreatedAt = now,
      Roles = roles,
      UserCredential = new UserCredential
      {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        UpdatedAt = now
      }
    };

    await _userRepo.AddAsync(user, ct);

    var accessResult = await _tokenService.GenerateAccessTokenAsync(user, [.. roles.Select(r => r.Name)], ct);
    var refreshResult = await _tokenService.GenerateRefreshTokenAsync(user.Id, null, now, ct);

    if (!accessResult.IsSuccess || !refreshResult.IsSuccess)
      return Result<AuthResponseDto>.Failure("Token generation failed."
      + accessResult.ErrorMessage + "\n"
      + refreshResult.ErrorMessage);

    var cookieResult = await _cookieService.AppendCookies(accessResult.Data!, refreshResult.Data!.UnhashedToken);
    if (!cookieResult.IsSuccess)
      return Result<AuthResponseDto>.Failure(cookieResult.ErrorMessage!);

    return Result<AuthResponseDto>.Success(new AuthResponseDto
    {
      AccessToken = accessResult.Data!,
      RefreshToken = refreshResult.Data!.UnhashedToken,
      RefreshTokenExpiresAt = refreshResult.Data!.Entity.ExpiresAt
    });
  }
}