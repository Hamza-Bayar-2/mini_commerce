using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string TokenString
) : ICommand<Result<AuthResponseDto>>;