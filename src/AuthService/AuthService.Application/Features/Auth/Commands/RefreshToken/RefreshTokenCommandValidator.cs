using FluentValidation;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.TokenString).NotEmpty().WithMessage("No refresh token string was injected");
    }
}