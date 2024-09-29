using domain.DatabaseObjects.Dtos;
using domain.Validators;
using domain.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;
using domain.Resources.Localizations;

namespace chldr_shared.Validators
{
    public class EntryValidator : AbstractValidator<EntryDto>
    {
        private readonly TranslationValidator _translationValidator;

        public EntryValidator(IStringLocalizer<AppLocalizations> stringLocalizer, TranslationValidator translationValidator)
        {
            _translationValidator = translationValidator;

            RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(stringLocalizer["Error:Content_must_be_set"]);

            RuleFor(x => x.Translations)
                 .Cascade(CascadeMode.Stop)
                 .NotNull()
                 .Must(translations => translations.All(IsValidTranslation))
                 .WithMessage((dto, translations) => GetTranslationsErrorMessage(translations, stringLocalizer))
                 .Must(translations => translations.Where(t => t.Rate > 0).GroupBy(t => t.LanguageCode).Where(group => group.Count() > 1).Count() == 0)
                 .WithMessage(stringLocalizer["Error:Only_one_translation_per_language_is_allowed"]);
        }

        private bool IsValidTranslation(TranslationDto translation)
        {
            var validationResult = _translationValidator.Validate(translation);
            return validationResult.IsValid;
        }

        private string GetTranslationsErrorMessage(IEnumerable<TranslationDto> translations, IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            var invalidTranslations = translations.Where(translation => !IsValidTranslation(translation)).ToList();

            if (invalidTranslations.Count > 0)
            {
                var invalidTranslation = invalidTranslations.First();
                var validationResult = _translationValidator.Validate(invalidTranslation);
                var error = validationResult.Errors.FirstOrDefault();
                if (error != null)
                {
                    return error.ErrorMessage;
                }
            }

            return stringLocalizer["Error:Invalid_translations"];
        }
    }
}