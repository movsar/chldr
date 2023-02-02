using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Dto;
using chldr_shared.Stores;
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
    public class WordEditViewModel : EntryEditViewModelBase<WordModel, WordValidator>
    {
        #region Properties
        [Parameter]
        public string? WordId { get; set; }
        public WordDto? Word { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(WordId))
            {
                return;
            }

            var wordId = new ObjectId(WordId);

            // Get current word from cached results
            var existingWord = ContentStore.CachedSearchResults.SelectMany(sr => sr.Entries)
                .Where(e => e.Type == EntryType.Word)
                .Cast<WordModel>()
                .FirstOrDefault(w => w.Id == wordId);

            if (existingWord == null)
            {
                throw new Exception("Error:Word_shouldn't_be_null");
            }

            Word = new WordDto(existingWord);

            InitializeViewModel(Word);
        }
    }
}
