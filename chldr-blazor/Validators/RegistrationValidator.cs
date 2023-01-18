using chldr_native.Resources.Localizations;
using chldr_native.ViewModels;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace chldr_native.Validators
{
    public class RegistrationValidator : AbstractValidator<RegistrationPageViewModel>
    {
        public RegistrationValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            RuleFor(x => x.Email)
                .NotNull()
                .Length(5, 100)
                .EmailAddress()
                .WithMessage(stringLocalizer["Error_EmailNotValid"]);

            RuleFor(x => x.Password)
                .NotNull()
                .Length(6, 10)
                .Equal(x => x.PasswordConfirmation)
                .WithMessage(stringLocalizer["Error_PasswordsDontMatch"]);

            RuleFor(x => x.Username)
                .Length(3, 16)
                .Matches("[A-zА-я0-9Ӏӏ]+")
                .WithMessage(stringLocalizer["Error_InvalidUsername"]);
        }
    }
}
