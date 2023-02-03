using chldr_data.Dto;
using chldr_data.Models;
using chldr_data.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
