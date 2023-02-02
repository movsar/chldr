using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationModel, TranslationValidator>
    {
        [Parameter]
        public string? WordId { get; set; }
        [Parameter]
        public string? TranslationId { get; set; }

        #region Fields and Properties
        public string Content { get; set; }
        public string Notes { get; set; }
        public string? LanguageCode { get; set; }
        #endregion

        public TranslationModel GetTranslation()
        {
            return new TranslationModel(Content, Notes,
                ContentStore.AllLanguages.First(l => l.Code == LanguageCode));
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(TranslationId))
            {
                return;
            }

            var wordId = new ObjectId(WordId);
            var translationId = new ObjectId(TranslationId);

            // Get current word from cached results
            var word = ContentStore.CachedSearchResults.SelectMany(sr => sr.Entries)
                .Where(e => e.Type == EntryType.Word)
                .Cast<WordModel>()
                .First(w => w.Id == wordId);

            // Get the current translation
            var translation = word.Translations.Where(t => t.Id == translationId).First();

            Content = translation.Content;
            Notes = translation.Notes;
            LanguageCode = translation.Language.Code;
        }

        internal void Submit()
        {
        }
    }
}
