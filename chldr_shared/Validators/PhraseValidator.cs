using chldr_data.Models;
using chldr_shared.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace chldr_shared.Validators
{
    public class PhraseValidator : AbstractValidator<PhraseModel>
    {
        public PhraseValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            RuleFor(x => x.Content)
            .NotNull()
            .WithMessage(stringLocalizer["Error:Content_must_be_set"]);
        }
    }
}