using chldr_ui.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public abstract class EditFormViewModelBase<TFormDto, TFormValidator> : ViewModelBase, IEditFormViewModel<TFormDto, TFormValidator>
        where TFormValidator : AbstractValidator<TFormDto>
    {
        [Inject] TFormValidator? DtoValidator { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public bool FormDisabled { get; set; }
        public bool FormSubmitted { get; set; }

        protected bool ExecuteSafely(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessages.Clear();
                ErrorMessages.Add(ex.Message);
                return false;
            }
        }
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

                ExceptionHandler?.LogError(ex); 
                return false;
            }
        }
        private bool Validate(TFormDto? formDto, string[]? validationRuleSets)
        {
            if (formDto == null)
            {
                throw new NullReferenceException("formDto is null!");
            }

            // Form validation
            ErrorMessages.Clear();

            ValidationResult? result;
            if (validationRuleSets == null)
            {
                result = DtoValidator?.Validate(formDto);
            }
            else
            {
                result = DtoValidator?.Validate(formDto, options => options.IncludeRuleSets(validationRuleSets));
            }

            if (result!.IsValid == false)
            {
                ErrorMessages.AddRange(result.Errors.Select(err => err.ErrorMessage));
                return false;
            }

            return true;
        }

        public void ValidateAndSubmit(TFormDto? formDto, Action action, string[]? validationRuleSets = null)
        {
            if (Validate(formDto, validationRuleSets) == false)
            {
                return;
            }

            // Block controls while processing
            FormDisabled = true;

            if (ExecuteSafely(action))
            {
                FormSubmitted = true;
            }

            FormDisabled = false;
            StateHasChanged();
        }
        public async Task ValidateAndSubmitAsync(TFormDto? formDto, Func<Task> func, string[]? validationRuleSets = null)
        {
            if (Validate(formDto, validationRuleSets) == false)
            {
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
