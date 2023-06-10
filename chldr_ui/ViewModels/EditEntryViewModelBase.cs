using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public abstract class EditEntryViewModelBase<TDto, TValidator> : EditFormViewModelBase<TDto, TValidator>
        where TValidator : AbstractValidator<TDto>
    {
        [Parameter] public string? EntryId { get; set; }
        // Set "User" source id by default
        protected string SourceId { get; set; } = "63a816205d1af0e432fba6de";
        protected bool IsEditMode = false;
    }
}
