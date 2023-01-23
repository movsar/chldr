using chldr_shared.Interfaces;
using chldr_shared.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_shared.ViewModels
{
    public abstract class EditFormViewModel<TFormDto, TFormValidator> : ViewModelBase, IEditFormViewModel<TFormDto, TFormValidator>
        where TFormValidator : AbstractValidator<TFormDto>
    {
        [Inject] TFormValidator? DtoValidator { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public bool FormDisabled { get; set; }
        public bool FormSubmitted { get; set; }
        public abstract Task ValidateAndSubmit();
        public async Task ValidateAndSubmit(TFormDto formDto, string[] validationRuleSets, Func<Task> func)
        {
            // Form validation
            var result = DtoValidator?.Validate(formDto, options => options.IncludeRuleSets(validationRuleSets));
            if (result!.IsValid == false)
            {
                ErrorMessages.AddRange(result.Errors.Select(err => err.ErrorMessage));
                return;
            }

            // Block controls while processing
            FormDisabled = true;

            try
            {
                await func();
            }
            catch (Exception ex)
            {
                FormDisabled = false;
                ErrorMessages.Add(ex.Message);
                StateHasChanged();

                return;
            }

            // Notify of success
            FormSubmitted = true;
            StateHasChanged();

            return;
        }
    }
}
