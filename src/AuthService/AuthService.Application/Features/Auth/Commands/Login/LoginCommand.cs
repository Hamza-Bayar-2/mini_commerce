using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<Result<AuthResponseDto>>;
