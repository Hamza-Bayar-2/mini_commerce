using FluentValidation;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
    }
}