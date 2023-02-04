using chldr_data.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public abstract class EntryEditViewModelBase<TFormDto, TFormValidator> : EditFormViewModelBase<TFormDto, TFormValidator>
        where TFormValidator : AbstractValidator<TFormDto>
    {
        #region Properties, Fields etc
        [Parameter]
        public ObjectId? EntryId { get; set; }
        public ObjectId? SourceId { get; set; }
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
