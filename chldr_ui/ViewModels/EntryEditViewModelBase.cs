using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Dto;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
