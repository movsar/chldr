using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using Org.BouncyCastle.Utilities;

namespace chldr_ui.ViewModels
{
    public class PhraseEditViewModel : EntryEditViewModelBase<PhraseDto, PhraseValidator>
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

            var existingPhrase = ContentStore.GetCachedPhraseById(new ObjectId(PhraseId));
            Phrase = new PhraseDto(existingPhrase);

            InitializeViewModel(Phrase);
        }

        private void SavePhrase()
        {
            ContentStore.UpdatePhrase(UserStore.CurrentUserInfo!, PhraseId, Phrase?.Content, Phrase?.Notes);
        }

        private void AddPhrase()
        {
            ContentStore.AddNewPhrase(UserStore.CurrentUserInfo!, Phrase?.Content!, Phrase?.Notes!);
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
