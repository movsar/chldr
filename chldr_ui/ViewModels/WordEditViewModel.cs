﻿using chldr_data.Dto;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.Models.Words;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : EntryEditViewModelBase<WordDto, WordValidator>
    {
        #region Properties
        [Inject] NavigationManager NavigationManager { get; set; }
        [Parameter]
        public string? wordId { get; set; }
        public WordDto? Word { get; set; }
        public int GrammaticalClass1 { get; set; }
        public int GrammaticalClass2 { get; set; }
        #endregion

        public void Save()
        {
            var wordId = this.wordId;

            if (Word == null)
            {
                var ex = new Exception("Word must not be empty");
                //_exceptionHandler.ProcessError(ex);
                throw ex;
            }

            UserModel user = null; // new UserModel(); // Get usermodel
            ContentStore.UpdateWord(user, Word);
            NavigationManager.NavigateTo("/");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(this.wordId))
            {
                return;
            }

            var wordId = this.wordId;

            // Get current word from cached results
            var existingWord = ContentStore.CachedSearchResult.Entries
                .Where(e => (EntryType)e.Type == EntryType.Word)
                .Cast<WordModel>()
                .FirstOrDefault(w => w.WordId == wordId);

            if (existingWord == null)
            {
                existingWord = ContentStore.GetWordById(wordId);
            }

            Word = new WordDto(existingWord);
            InitializeViewModel(Word);
        }
    }
}
