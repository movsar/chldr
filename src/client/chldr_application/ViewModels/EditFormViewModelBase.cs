using core.DatabaseObjects.Models;
using DynamicData;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace chldr_application.ViewModels
{
    public abstract class EditFormViewModelBase<TFormDto, TFormValidator> : ViewModelBase
        where TFormValidator : AbstractValidator<TFormDto>
    {
        protected TFormValidator? DtoValidator { get; set; }

        private ObservableCollection<string> _errorMessages = new ObservableCollection<string>();
        public ObservableCollection<string> ErrorMessages {
            get => _errorMessages;
            set => this.RaiseAndSetIfChanged(ref _errorMessages, value);
        } 

        public bool FormSubmitted { get; set; } = false;

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

            if (result!.IsValid == false) { 
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

            await ExecuteSafelyAsync(func);
        }
    }
}
