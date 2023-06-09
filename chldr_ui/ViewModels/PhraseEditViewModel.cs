using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class PhraseEditViewModel : EditEntryViewModelBase<PhraseDto, PhraseValidator>
    {
        #region Properties, Fields and Events
        [Parameter]
        public string? PhraseId { get; set; }
        public PhraseDto? Phrase { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (string.IsNullOrEmpty(PhraseId))
            {
                // New entry mode
                Phrase = new PhraseDto();
                return;
            }

            // Edit entry mode
            IsEditMode = true;

            var existingPhrase = ContentStore.GetCachedPhraseById(PhraseId);
            Phrase = PhraseDto.FromModel(existingPhrase);
            SourceId = Phrase.SourceId;
        }

        private void UpdatePhrase()
        {
            ContentStore.UpdatePhrase(UserModel.FromDto(UserStore.ActiveSession.User!), Phrase);
        }

        private void InsertPhrase()
        {
            ContentStore.AddPhrase(UserModel.FromDto(UserStore.ActiveSession.User!), Phrase);
        }

        public void Submit()
        {
            if (IsEditMode)
            {
                ValidateAndSubmit(Phrase, UpdatePhrase);
            }
            else
            {
                ValidateAndSubmit(Phrase, InsertPhrase);
            }
        }
    }
}
