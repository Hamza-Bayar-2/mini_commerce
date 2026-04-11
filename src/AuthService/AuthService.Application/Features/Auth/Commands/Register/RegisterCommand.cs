using AuthService.Application.Common.Models;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber) : ICommand<Result<AuthResponseDto>>;