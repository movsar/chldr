using chldr_data.Models;
using chldr_shared.Resources.Localizations;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.Validators
{
    public class WordValidator : AbstractValidator<WordModel>
    {
        public WordValidator(IStringLocalizer<AppLocalizations> stringLocalizer)
        {
            RuleFor(x => x.Content)
            .NotNull()
            .WithMessage(stringLocalizer["Error:Word_content_must_be_set"]);

            //RuleFor(x => x.Language)
            //.NotNull()
            //.WithMessage(stringLocalizer["Error:Translation_language_must_be_set"]);

            //RuleFor(x => x.Content)
            //.NotEmpty()
            //.WithMessage(stringLocalizer["Error:Translation_text_must_be_set"]);
        }
    }
}
