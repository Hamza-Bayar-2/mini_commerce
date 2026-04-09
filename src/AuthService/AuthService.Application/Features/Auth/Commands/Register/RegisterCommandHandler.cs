

using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
  private readonly IUserRepository _userRepo;
  private readonly IRoleRepository _roleRepo;
  private readonly ITokenService _tokenService;
  private readonly IUnitOfWork _unitOfWork;

  public RegisterCommandHandler(IUserRepository userRepo, IRoleRepository roleRepo, ITokenService tokenService, IUnitOfWork unitOfWork)
  {
    _userRepo = userRepo;
    _roleRepo = roleRepo;
    _tokenService = tokenService;
    _unitOfWork = unitOfWork;
  }

  public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken ct)
  {
    var existing = await _userRepo.GetByEmailAsync(request.Email, ct);
    if (existing is not null)
      throw new Exception("Email already exists.");

    var now = DateTime.UtcNow;

    var roles = await _roleRepo.GetRolesByNamesAsync(["customer"], ct);

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
      throw new Exception("Token generation failed.");

    await _unitOfWork.SaveChangesAsync(ct);

    return new AuthResponseDto
    {
      AccessToken = accessResult.Data!,
      RefreshToken = refreshResult.Data.UnhashedToken,
      RefreshTokenExpiresAt = refreshResult.Data.Entity.ExpiresAt
    };
  }
}