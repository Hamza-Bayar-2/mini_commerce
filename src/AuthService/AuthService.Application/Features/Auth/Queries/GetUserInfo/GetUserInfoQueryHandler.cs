using AuthService.Application.DTOs;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using MediatR;

namespace AuthService.Application.Features.Auth.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfoResponseDto>
{
  private readonly IUserRepository _userRepo;
  private readonly ICookieService _cookieService;

  public GetUserInfoQueryHandler(IUserRepository userRepo, ICookieService cookieService)
  {
    _userRepo = userRepo;
    _cookieService = cookieService;
  }

  public async Task<UserInfoResponseDto> Handle(GetUserInfoQuery request, CancellationToken ct)
  {
    var userId = _cookieService.GetUserId();

    if (userId == null)
      throw new Exception("Unauthorized access.");

    var user = await _userRepo.GetUserInfoAsync(userId.Value, ct);

    if (user == null)
      throw new Exception("User not found.");

    return new UserInfoResponseDto
    {
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      PhoneNumber = user.PhoneNumber,
      Roles = user.Roles.Select(r => r.Name).ToList()
    };
  }
}