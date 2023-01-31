using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Models;
using chldr_shared.Dto;
using chldr_shared.Validators;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    internal class PhraseEditViewModel : EntryEditViewModelBase<PhraseModel, PhraseValidator>
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

            InitializeViewModel(phrase);
        }
        public override async Task ValidateAndSubmit()
        {
            await ValidateAndSubmit(Phrase, new string[] { "Email" }, SendPasswordResetRequest);

            if (IsEditMode)

            {
                ContentStore.UpdatePhrase(UserStore.CurrentUserInfo, PhraseId, Content, Notes);
            }
            else
            {
                ContentStore.AddNewPhrase(Content, Notes);
            }
        }
    }
}
