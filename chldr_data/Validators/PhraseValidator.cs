using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace chldr_shared.Validators
{
    public class PhraseValidator : AbstractValidator<PhraseDto>
    {
        public PhraseValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            RuleFor(x => x.Content)
            .NotNull()
            .WithMessage(stringLocalizer["Error:Content_must_be_set"]);
        }
    }
}