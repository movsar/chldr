using chldr_data.Dto;
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

        private TranslationDto CreateTranslationDto(ObjectId entryId, ObjectId translationId)
        {
            var entry = ContentStore.CachedSearchResult.Entries
                .First(e => e.Id == entryId);

            return new TranslationDto(entry.Translations.First(t => t.Id == translationId));
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
