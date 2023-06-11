using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Resources.Localizations;
using chldr_data.Validators;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace chldr_shared.Validators
{
    public class WordValidator : AbstractValidator<EntryDto>
    {
        private readonly TranslationValidator _translationValidator;

        public WordValidator(IStringLocalizer<AppLocalizations> stringLocalizer, TranslationValidator translationValidator)
        {
            _translationValidator = translationValidator;

            RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(stringLocalizer["Error:Word_content_must_be_set"]);

            RuleFor(x => x.Translations)
                 .Cascade(CascadeMode.Stop)
                 .NotNull()
                 .WithMessage(stringLocalizer["Error:There_must_be_at_least_one_translation"])
                 .Must(translations => translations.All(IsValidTranslation))
                 .WithMessage((dto, translations) => GetTranslationsErrorMessage(translations, stringLocalizer));
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