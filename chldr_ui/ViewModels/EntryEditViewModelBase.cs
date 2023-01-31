using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
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
        public string? Source { get; set; }
        public List<TranslationModel> Translations { get; } = new();
        protected bool IsEditMode = false;
        #endregion
        protected virtual void InitializeViewModel(EntryModel entry)
        {
            Translations.AddRange(entry.Translations);
            Source = entry.Source.Name;
        }

        public override async Task ValidateAndSubmit() { }
    }
}
