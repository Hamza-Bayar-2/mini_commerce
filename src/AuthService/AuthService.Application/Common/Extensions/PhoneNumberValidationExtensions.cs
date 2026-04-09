using FluentValidation;
using AuthService.Application.Common.Validators;

namespace AuthService.Application.Common.Extensions;

public static class PhoneNumberValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> IsValidPhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new PhoneNumberValidator<T>());
    }
}
