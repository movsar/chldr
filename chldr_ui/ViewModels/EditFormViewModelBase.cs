using chldr_shared;
using chldr_ui.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public abstract class EditFormViewModelBase<TFormDto, TFormValidator> : ViewModelBase, IEditFormViewModel<TFormDto, TFormValidator>
        where TFormValidator : AbstractValidator<TFormDto>
    {
        [Inject] JsInteropService JsInterop { get; set; }
        [Inject] TFormValidator? DtoValidator { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
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

        public async Task ValidateAndSubmitAsync(TFormDto? formDto, Func<Task> func, string[]? validationRuleSets = null)
        {
            if (Validate(formDto, validationRuleSets) == false)
            {
                return;
            }

            // Block controls
            await JsInterop.Disable("[data-id=form_container]");

            // Process
            if (await ExecuteSafelyAsync(func))
            {
                FormSubmitted = true;
            }

            // Unblock controls
            await JsInterop.Enable("[data-id=form_container]");

            await RefreshUi();
        }
    }
}
