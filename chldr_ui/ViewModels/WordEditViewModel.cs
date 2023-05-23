using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_data.DatabaseObjects.Models;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : EditEntryViewModelBase<WordDto, WordValidator>
    {
        private bool isInitialized = false;
        public WordDto Word { get; set; } = new WordDto();
        protected override void OnInitialized()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                if (string.IsNullOrEmpty(EntryId))
                {
                    return;
                }

                // Get current word from cached results
                var existingWord = ContentStore.CachedSearchResult.Entries
                    .Where(e => (EntryType)e.Type == EntryType.Word)
                    .Cast<WordModel>()
                    .FirstOrDefault(w => w.EntryId == this.EntryId);

                if (existingWord == null)
                {
                    existingWord = (WordModel)ContentStore.GetEntryById(EntryId);
                }

                Word = WordDto.FromModel(existingWord);
                SourceId = Word.SourceId;
            }
        }

        List<string> _newTranslationIds = new List<string>();
        public async Task NewTranslation()
        {
            var translation = new TranslationDto();
            // Needed to know which translations are new, in case they need to be removed
            _newTranslationIds.Add(translation.TranslationId);

            Word.Translations.Add(translation);

            await RefreshUi();
        }
        public async Task DeleteTranslation(string translationId)
        {
            if (!_newTranslationIds.Contains(translationId))
            {
                // TODO: Remove from the database
            }
            else
            {
                _newTranslationIds.Remove(translationId);
            }

            Word.Translations.Remove(Word.Translations.Find(t => t.TranslationId.Equals(translationId))!);
            await RefreshUi();
        }

        public async Task Save()
        {
            if (Word == null)
            {
                var ex = new Exception("Word must not be empty");
                //_exceptionHandler.ProcessError(ex);
                throw ex;
            }

            await ContentStore.UpdateWord(UserModel.FromDto(UserStore.ActiveSession.User!), Word);
            NavigationManager.NavigateTo("/");
        }
    }
}
