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
        #region Properties and Parameters
        [Parameter]
        public string? Id { get; set; }
        public string? Source { get; set; }
        public List<TranslationModel> Translations { get; } = new();
        #endregion
        protected abstract void InitializeViewModel(EntryModel entry);
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(Id))
            {
                return;
            }

            var entry = ContentStore.GetEntryById(new ObjectId(Id));
            if (entry == null)
            {
                throw new Exception("Error:entry_should_not_be_empty");
            }

            Translations.AddRange(entry.Translations);
            Source = entry.Source.Name;

            InitializeViewModel(entry);
        }

        public override async Task ValidateAndSubmit() { }
    }
}
