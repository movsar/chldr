using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace chldr_data.Validators
{
    public class TranslationValidator : AbstractValidator<TranslationDto>
    {
        public TranslationValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {

            RuleFor(x => x.LanguageCode)
            .NotNull()
            .WithMessage(stringLocalizer["Error:Translation_language_must_be_set"]);

            RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(stringLocalizer["Error:Translation text must be set"]);
        }
    }
}
