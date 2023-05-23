using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public abstract class EditEntryViewModelBase<TDto, TValidator> : EditFormViewModelBase<TDto, TValidator>
        where TValidator : AbstractValidator<TDto>
    {
        [Parameter]
        public string? EntryId { get; set; }
        public string? SourceId { get; set; }
        protected bool IsEditMode = false;
    }
}
