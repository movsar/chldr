using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_data.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums.WordDetails;
using chldr_shared;
using Blazored.Modal;
using chldr_ui.Components;

namespace chldr_ui.ViewModels
{
    public class EntryEditViewModel : EditFormViewModelBase<EntryDto, EntryValidator>
    {
        [Parameter]
        public string? EntryId { get; set; }
        public EntryDto EntryDto { get; set; } = new EntryDto();
        protected string SourceId { get; set; } = "63a816205d1af0e432fba6de";
        internal EntryType SelectedEntryType { get; set; } = EntryType.Word;
        internal void HandleEntryTypeChange(ChangeEventArgs e)
        {
            if (Enum.TryParse(e.Value?.ToString(), out EntryType selectedEntryType))
            {
                SelectedEntryType = selectedEntryType;
            }
        }

        #region Lifecycle Event Handlers
        private bool isInitialized = false;
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
                var existingEntry = ContentStore.CachedSearchResult.Entries
                    .Where(e => e.Type == EntryType.Word)
                    .Cast<EntryModel>()
                    .FirstOrDefault(w => w.EntryId == EntryId);

                if (existingEntry == null)
                {
                    existingEntry = ContentStore.GetByEntryId(EntryId);
                }

                EntryDto = EntryDto.FromModel(existingEntry);
            }
        }
        #endregion

        #region Translation Handlers
        List<string> _newTranslationIds = new List<string>();
        public async Task NewTranslation()
        {
            var translation = new TranslationDto(EntryDto.EntryId, UserStore.ActiveSession.User!.UserId, ContentStore.Languages.First());

            // Needed to know which translations are new, in case they need to be removed
            _newTranslationIds.Add(translation.TranslationId);

            EntryDto.Translations.Add(translation);

            await RefreshUi();
        }
        public async Task DeleteTranslation(string translationId)
        {
            if (_newTranslationIds.Contains(translationId))
            {
                _newTranslationIds.Remove(translationId);
            }

            EntryDto.Translations.Remove(EntryDto.Translations.Find(t => t.TranslationId.Equals(translationId))!);
            await RefreshUi();
        }
        #endregion

        #region Form Actions

        bool isRecording = false;
        public async Task ToggleRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                await JsInterop.StopRecording();
            }
            else
            {
                isRecording = true;
                await JsInterop.StartRecording();
            }
        }
        public async Task SaveClickHandler()
        {
            if (EntryDto.Translations.Count() == 0)
            {
                if (await AskForConfirmation("Question:Do_you_really_want_to_add_new_entry_without_a_translation?") != true)
                {
                    return;
                }
            }

            await ValidateAndSubmitAsync(EntryDto, Save);
        }

        public async Task Save()
        {
            var user = UserModel.FromDto(UserStore.ActiveSession.User);
            EntryDto.UserId = user.UserId;
            EntryDto.SourceId = SourceId;

            if (EntryDto.CreatedAt != DateTimeOffset.MinValue)
            {
                await ContentStore.UpdateEntry(user, EntryDto);
            }
            else
            {
                await ContentStore.AddEntry(user, EntryDto);
            }

            NavigationManager.NavigateTo("/");
        }
        #endregion
    }
}
