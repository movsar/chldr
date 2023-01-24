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
        public bool FormDisabled { get; set; } = false;
        public bool FormSubmitted { get; set; } = false;
        public abstract Task ValidateAndSubmit();
        protected async Task<bool> ExecuteSafelyAsync(Func<Task> func)
        {
            try
            {
                await func();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessages.Clear();
                ErrorMessages.Add(ex.Message);
                return false;
            }
        }
        public async Task ValidateAndSubmit(TFormDto formDto, string[] validationRuleSets, Func<Task> func)
        {
            if (formDto == null)
            {
                throw new NullReferenceException("formDto is null!");
            }

            // Form validation
            ErrorMessages.Clear();
            var result = DtoValidator?.Validate(formDto, options => options.IncludeRuleSets(validationRuleSets));
            if (result!.IsValid == false)
            {
                ErrorMessages.AddRange(result.Errors.Select(err => err.ErrorMessage));
                return;
            }

            // Block controls while processing
            FormDisabled = true;

            if (await ExecuteSafelyAsync(func))
            {
                FormSubmitted = true;
            }

            FormDisabled = false;
            StateHasChanged();
        }
    }
}
