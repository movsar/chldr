using core.DatabaseObjects.Dtos;
using core.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace core.Validators
{
    public class TranslationValidator : AbstractValidator<TranslationDto>
    {
        public TranslationValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            RuleFor(x => x.LanguageCode)
            .NotNull()
            .WithMessage(stringLocalizer["Error:Translation_language_must_be_set"]);

            //RuleFor(x => x.EntryId)
            //.NotEmpty()
            //.WithMessage(stringLocalizer["Error:Entry_is_not_specified"]);

            RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(stringLocalizer["Error:Translation_text_must_be_set"]);
        }
    }
}
