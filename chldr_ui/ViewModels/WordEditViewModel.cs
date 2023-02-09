using chldr_data.Dto;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Stores;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : EntryEditViewModelBase<WordDto, WordValidator>
    {
        #region Properties
        [Parameter]
        public string? wordId { get; set; }
        public WordDto? Word { get; set; }
        #endregion

        public void Save()
        {
            var wordId = new ObjectId(this.wordId);
            ContentStore.UpdateWord(wordId, Word);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(this.wordId))
            {
                return;
            }

            var wordId = new ObjectId(this.wordId);

            // Get current word from cached results
            var existingWord = ContentStore.CachedSearchResults.SelectMany(sr => sr.Entries)
                .Where(e => (EntryType)e.Type == EntryType.Word)
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
