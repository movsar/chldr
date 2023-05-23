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

        private void SavePhrase()
        {
            ContentStore.UpdatePhrase(UserModel.FromDto(UserStore.ActiveSession.User!), PhraseId, Phrase?.Content, Phrase?.Notes);
        }

        private void AddPhrase()
        {
            ContentStore.AddNewPhrase(UserModel.FromDto(UserStore.ActiveSession.User!), Phrase?.Content!, Phrase?.Notes!);
        }

        public void Submit()
        {
            if (IsEditMode)
            {
                ValidateAndSubmit(Phrase, SavePhrase);
            }
            else
            {
                ValidateAndSubmit(Phrase, AddPhrase);
            }
        }
    }
}
