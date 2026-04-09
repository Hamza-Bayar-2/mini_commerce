using FluentValidation;
using AuthService.Application.Common.Extensions;

namespace AuthService.Application.Features.Auth.Commands.Register;


public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithMessage("Please specify a first name");
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithMessage("Please specify a last name");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Please enter a valid email address");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password);
        RuleFor(x => x.PhoneNumber).IsValidPhoneNumber();
    }
}