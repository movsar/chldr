using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_data.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;
using chldr_utils.Services;
using chldr_shared;
using chldr_data.Enums.WordDetails;

namespace chldr_ui.ViewModels
{
    public class EntryEditViewModel : EditFormViewModelBase<EntryDto, EntryValidator>
    {

        public EntryEditViewModel()
        {
            JsInteropService.OnRemoveAudio = (recordingId) =>
            {
                var removedSound = EntryDto.Sounds.FirstOrDefault(s => s.SoundId.Equals(recordingId));
                if (removedSound == null)
                {
                    throw ExceptionHandler!.Error("Sound doesn't exist in the dto");
                }

                EntryDto.Sounds.Remove(removedSound);
            };
        }

        [Parameter] public string? EntryId { get; set; }
        [Inject] FileService FileService { get; set; }
        public EntryDto EntryDto { get; set; } = new EntryDto();
        protected string SourceId { get; set; } = "63a816205d1af0e432fba6de";
        bool isRecording;
        bool existingSoundsRendered;
        SoundDto latestSoundDto;

        private async Task RenderExistingSounds()
        {
            foreach (var soundDto in EntryDto.Sounds)
            {

                await JsInterop.AddExistingEntryRecording(soundDto);
            }

            existingSoundsRendered = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!existingSoundsRendered)
            {
                await RenderExistingSounds();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (string.IsNullOrEmpty(EntryId))
            {
                await NewTranslation();
                return;
            }

            // Load the EntryDto for the existing entry being edited
            var existingEntry = ContentStore.CachedSearchResult.Entries
                .Where(e => e.Type == EntryType.Word)
                .Cast<EntryModel>()
                .FirstOrDefault(w => w.EntryId == EntryId);

            if (existingEntry == null)
            {
                existingEntry = await ContentStore.GetByEntryId(EntryId);
            }
            EntryDto = EntryDto.FromModel(existingEntry);

            // Load audio recordings, if any
            foreach (var soundDto in EntryDto.Sounds)
            {
                var soundPath = Path.Combine(FileService.EntrySoundsDirectory, soundDto.FileName);
                if (!File.Exists(soundPath))
                {
                    // TODO: Retireve
                    continue;
                }

                soundDto.RecordingB64 = File.ReadAllText(soundPath);
            }

            await base.OnParametersSetAsync();
        }

        #region Translation Handlers
        List<string> _newTranslationIds = new List<string>();
        public async Task NewTranslation()
        {
            var translation = new TranslationDto(EntryDto.EntryId, UserStore.CurrentUser!.UserId, ContentStore.Languages.First());

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

        private async Task StartRecording()
        {
            isRecording = true;
            latestSoundDto = new SoundDto();

            latestSoundDto.EntryId = EntryDto.EntryId!;
            latestSoundDto.UserId = UserStore.CurrentUser!.UserId;

            await JsInterop.StartRecording(latestSoundDto.SoundId);
        }
        public async Task ToggleRecording()
        {
            if (isRecording)
            {
                await StopRecording();
            }
            else
            {
                await StartRecording();
            }
            await RefreshUi();
        }

        private async Task StopRecording()
        {
            isRecording = false;
            var recording = await JsInterop.StopRecording();
            if (recording == null || string.IsNullOrEmpty(recording))
            {
                return;
            }

            latestSoundDto.RecordingB64 = recording;
            EntryDto.Sounds.Add(latestSoundDto);
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
            var user = UserStore.CurrentUser;
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
