using chldr_data.DatabaseObjects.Dtos;
using chldr_data.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class TranslationEditViewModel : EditFormViewModelBase<TranslationDto, TranslationValidator>
    {
        #region Fields and Properties
        [Parameter]
        public TranslationDto Translation { get; set; } = new TranslationDto();
        public string LanguageCode { get; set; }
        #endregion

        [Parameter]
        public Action<string> OnDelete { get; set; }
        private TranslationDto CreateTranslationDto(string entryId, string translationId)
        {
            var entry = ContentStore.CachedSearchResult.Entries
                .First(e => e.EntryId == entryId);

            return TranslationDto.FromModel(entry.Translations.First(t => t.TranslationId == translationId));
        }

        public void Delete()
        {
            OnDelete?.Invoke(Translation.TranslationId);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Translation == null)
            {
                return;
            }

            // Init code
        }

        internal void Submit()
        {

        }
    }
}
