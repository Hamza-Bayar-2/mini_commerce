using AuthService.Application.Common.Models;
using AuthService.Application.Interfaces;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Logout;

public record LogoutCommand() : ICommand<Result<Unit>>;
