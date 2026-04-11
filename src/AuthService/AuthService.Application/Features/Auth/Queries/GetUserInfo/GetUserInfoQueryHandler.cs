using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using MediatR;

namespace AuthService.Application.Features.Auth.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Result<UserInfoResponseDto>>
{
    private readonly IUserRepository _userRepo;
    private readonly ICookieService _cookieService;

    public GetUserInfoQueryHandler(IUserRepository userRepo, ICookieService cookieService)
    {
        _userRepo = userRepo;
        _cookieService = cookieService;
    }

    public async Task<Result<UserInfoResponseDto>> Handle(GetUserInfoQuery request, CancellationToken ct)
    {
        var userId = _cookieService.GetUserId();

        if (userId == null)
            return Result<UserInfoResponseDto>.Failure("Unauthorized access.");

        var user = await _userRepo.GetUserInfoAsync(userId.Value, ct);

        if (user == null)
            return Result<UserInfoResponseDto>.Failure("User not found.");

        return Result<UserInfoResponseDto>.Success(new UserInfoResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles = user.Roles.Select(r => r.Name).ToList()
        });
    }
}