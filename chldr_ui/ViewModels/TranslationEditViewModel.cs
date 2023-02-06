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
        public string? EntryId { get; set; }
        [Parameter]
        public string? TranslationId { get; set; }
        public TranslationDto? Translation { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(TranslationId) || string.IsNullOrEmpty(EntryId))
            {
                return;
            }

            var entryId = new ObjectId(EntryId);
            var entry = ContentStore.CachedSearchResults.SelectMany(sr => sr.Entries)
                .First(e => e.Id == entryId);

            var translationId = new ObjectId(TranslationId);
            Translation = new TranslationDto(entry.Translations.First(t => t.Id == translationId));
        }

        internal void Submit()
        {
        }
    }
}
