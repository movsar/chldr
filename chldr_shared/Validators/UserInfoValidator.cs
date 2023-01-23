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
            RuleSet("Password", () =>
            {
                RuleFor(x => x.Password)
                .NotEmpty()
                .Length(6, 100)
                .WithMessage(stringLocalizer["Error:Passwords_is_not_valid"]);

                RuleFor(x => x.Password)
                .Equal(x => x.PasswordConfirmation)
                .WithMessage(stringLocalizer["Error:Passwords_dont_match"])
                .Unless(x => string.IsNullOrEmpty(x.PasswordConfirmation));
            });

            RuleSet("Name", () =>
            {
                RuleFor(x => x.FirstName)
                  .Length(2, 30)
                  .Matches("^[A-zА-я0-9Ӏӏ]+$")
                  .WithMessage(stringLocalizer["Error:Invalid_name"])
                  .Unless(x => string.IsNullOrWhiteSpace(x.FirstName));

                RuleFor(x => x.LastName)
                   .Length(2, 30)
                   .Matches("^[A-zА-я0-9Ӏӏ]+$")
                   .WithMessage(stringLocalizer["Error:Invalid_name"])
                   .Unless(x => string.IsNullOrWhiteSpace(x.LastName));
            });

            RuleSet("Email", () =>
            {
                RuleFor(x => x.Email)
                .NotEmpty()
                .Length(5, 100)
                .EmailAddress()
                .WithMessage(stringLocalizer["Error:Email_not_valid"]);
            });
        }
    }
}
