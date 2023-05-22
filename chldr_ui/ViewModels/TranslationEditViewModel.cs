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
        public TranslationDto? Translation { get; set; }
        #endregion

        private TranslationDto CreateTranslationDto(string entryId, string translationId)
        {
            var entry = ContentStore.CachedSearchResult.Entries
                .First(e => e.EntryId == entryId);

            return TranslationDto.FromModel(entry.Translations.First(t => t.TranslationId == translationId));
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
