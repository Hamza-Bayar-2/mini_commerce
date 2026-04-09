using FluentValidation;
using FluentValidation.Validators;
using PhoneNumbers;

namespace AuthService.Application.Common.Validators;

public class PhoneNumberValidator<T> : PropertyValidator<T, string?>
{
    public override string Name => "PhoneNumberValidator";

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        if (!value.StartsWith('+'))
            return false;

        var phoneUtil = PhoneNumberUtil.GetInstance();

        try
        {
            var phoneNumber = phoneUtil.Parse(value, null);
            return phoneUtil.IsValidNumber(phoneNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
      => "{PropertyName} must be a valid international phone number starting with '+' (e.g., +905554443322).";
}
