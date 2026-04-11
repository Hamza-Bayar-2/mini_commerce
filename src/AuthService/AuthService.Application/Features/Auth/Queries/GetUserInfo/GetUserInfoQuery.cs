using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Features.Auth.Queries.GetUserInfo;

public record GetUserInfoQuery() : IQuery<Result<UserInfoResponseDto>>;