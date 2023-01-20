using chldr_shared.ViewModels;
using chldr_shared.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;
using chldr_shared.Dto;

namespace chldr_shared.Validators
{
    public class UserInfoValidator : AbstractValidator<UserInfoDto>
    {
        public UserInfoValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            RuleFor(x => x.Email)
                .NotNull()
                .Length(5, 100)
                .EmailAddress()
                .WithMessage(stringLocalizer["Error:Email_not_valid"]);

            RuleFor(x => x.Password)
                .Length(6, 30)
                .Equal(x => x.PasswordConfirmation)
                .WithMessage(stringLocalizer["Error:Passwords_dont_match"]);

            RuleFor(x => x.Username)
                .Length(3, 16)
                .Matches("[A-zА-я0-9Ӏӏ]+")
                .WithMessage(stringLocalizer["Error:Invalid_username"]);

            RuleFor(x => x.FirstName)
                .Length(3, 16)
                .Matches("[A-zА-я- Ӏӏ]+")
                .WithMessage(stringLocalizer["Error:Invalid_name"]);
        }
    }
}
