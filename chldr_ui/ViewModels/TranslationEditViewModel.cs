using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationDto, TranslationValidator>
    {
        #region Fields and Properties
        [Parameter]
        public string? EntryId { get; set; }
        [Parameter]
        public string? TranslationId { get; set; }
        public TranslationDto Translation { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(TranslationId))
            {
                return;
            }

            //var wordId = new ObjectId(WordId);
            //var translationId = new ObjectId(TranslationId);

            //// Get current word from cached results
            //var word = ContentStore.CachedSearchResults.SelectMany(sr => sr.Entries)
            //    .Where(e => e.Type == EntryType.Word)
            //    .Cast<WordModel>()
            //    .First(w => w.Id == wordId);

            //// Get the current translation
            //var translation = word.Translations.Where(t => t.Id == translationId).First();

            //Content = translation.Content;
            //Notes = translation.Notes;
            //LanguageCode = translation.Language.Code;
        }

        internal void Submit()
        {
        }
    }
}
