using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public abstract class EditEntryViewModelBase<TDto, TValidator> : EditFormViewModelBase<TDto, TValidator>
        where TValidator : AbstractValidator<TDto>
    {
        #region Properties, Fields etc
        [Parameter]
        public string? EntryId { get; set; }
        public string? SourceId { get; set; }
        public List<TranslationDto> Translations { get; } = new();
        protected bool IsEditMode = false;
        #endregion
        protected virtual void InitializeViewModel(EntryDto entry)
        {
            Translations.AddRange(entry.Translations);
            SourceId = entry.SourceId;
        }
    }
}
