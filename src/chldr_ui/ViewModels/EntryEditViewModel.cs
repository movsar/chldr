using core.Enums;
using core.DatabaseObjects.Dtos;
using chldr_shared.Validators;
using core.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;
using chldr_utils.Services;
using chldr_shared;
using core.Services;
using chldr_application.Services;

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

            JsInteropService.OnPromoteAudio = (recordingId) =>
            {
                var pronunciation = EntryDto.Sounds.FirstOrDefault(s => s.SoundId.Equals(recordingId));
                if (pronunciation == null)
                {
                    throw ExceptionHandler!.Error("Sound doesn't exist in the dto");
                }

                Task.Run(() => PromotePronunciationAsync(pronunciation));
            };
        }

        [Parameter] public string? EntryId { get; set; }
        [Inject] FileService FileService { get; set; }
        public EntryDto EntryDto { get; set; } = new EntryDto();
        bool isRecording;
        bool existingSoundsRendered;
        PronunciationDto latestSoundDto;
        List<string> _newTranslationIds = new List<string>();
        internal bool CanEditEntry { get; private set; } = true;

        private async Task RenderExistingSounds()
        {
            if (UserStore?.CurrentUser == null)
            {
                throw new NullReferenceException("Current user should not be null");
            }
            foreach (var soundDto in EntryDto.Sounds)
            {
                bool canPromote = UserStore.CurrentUser.CanPromote(soundDto.Rate, soundDto.UserId);
                bool canRemove = UserStore.CurrentUser.CanRemove(soundDto.Rate, soundDto.UserId, soundDto.CreatedAt);

                await JsInterop.AddExistingEntryRecording(soundDto, canPromote, canRemove);
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
            if (UserStore.CurrentUser == null)
            {
                ExceptionHandler?.LogError("EntryEditViewModel: OnParametersSetAsync - CurrentUser is null");
                NavigationManager.NavigateTo("/");
            }

            if (string.IsNullOrEmpty(EntryId))
            {
                await NewTranslationAsync();
                return;
            }

            // Load the EntryDto for the existing entry being edited
            var existingEntry = /*ContentStore.EntryService.GetRandomsAsync()*/ new List<EntryModel>()
                .Where(e => e.Type == EntryType.Word)
                .Cast<EntryModel>()
                .FirstOrDefault(w => w.EntryId == EntryId);

            if (existingEntry == null)
            {
                existingEntry = await ContentStore.EntryService.GetAsync(EntryId);
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

            CanEditEntry = UserStore.CurrentUser?.CanEdit(EntryDto.Rate, EntryDto.UserId!) == true;

            await base.OnParametersSetAsync();
        }

        #region Translation Handlers

        public async Task NewTranslationAsync()
        {
            if (UserStore.CurrentUser == null)
            {
                ExceptionHandler?.LogError("EntryEditViewModel: NewTranslation - CurrentUser is null");
                NavigationManager.NavigateTo("/");
            }

            var translation = new TranslationDto(EntryDto.EntryId, UserStore.CurrentUser!.Id, ContentStore.Languages.First());

            // Needed to know which translations are new, in case they need to be removed
            _newTranslationIds.Add(translation.TranslationId);

            EntryDto.Translations.Add(translation);

            await RefreshUiAsync();
        }
        public async Task PromoteEntryAsync(EntryDto entryDto)
        {
            await ContentStore.EntryService.PromoteAsync(entryDto, UserStore.CurrentUser);
        }

        public async Task PromoteTranslationAsync(TranslationDto translationDto)
        {
            await ContentStore.EntryService.PromoteTranslationAsync(translationDto, UserStore.CurrentUser);
        }

        public async Task PromotePronunciationAsync(PronunciationDto soundDto)
        {
            await ContentStore.EntryService.PromotePronunciationAsync(soundDto, UserStore.CurrentUser);
        }

        public async Task DeleteTranslationAsync(string translationId)
        {
            var translationDto = EntryDto.Translations.Find(t => t.TranslationId.Equals(translationId))!;
            if (!(UserStore.CurrentUser?.CanRemove(translationDto.Rate, translationDto.UserId, translationDto.CreatedAt) == true))
            {
                return;
            }

            if (_newTranslationIds.Contains(translationId))
            {
                _newTranslationIds.Remove(translationId);
            }

            EntryDto.Translations.Remove(translationDto);
            await RefreshUiAsync();
        }
        #endregion

        #region Form Actions


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
            //await RefreshUi();
        }
        private async Task StartRecording()
        {
            isRecording = true;
            latestSoundDto = new PronunciationDto();
            latestSoundDto.EntryId = EntryDto.EntryId!;
            latestSoundDto.Rate = UserStore.CurrentUser!.GetRateRange().Lower;
            latestSoundDto.UserId = UserStore.CurrentUser!.Id;

            await JsInterop.StartRecording(latestSoundDto.SoundId);
        }

        private async Task StopRecording()
        {
            isRecording = false;

            try
            {
                var recording = await JsInterop.StopRecording();
                if (recording == null || string.IsNullOrEmpty(recording))
                {
                    return;
                }

                latestSoundDto.RecordingB64 = recording;
                EntryDto.Sounds.Add(latestSoundDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping recording: " + ex.Message);
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
            var user = UserStore.CurrentUser;
            if (user == null)
            {
                throw new NullReferenceException("CurrentUser must not be null");
            }

            if (EntryDto.CreatedAt != DateTimeOffset.MinValue)
            {
                await ContentStore.EntryService.UpdateAsync(EntryDto, user.Id);
            }
            else
            {
                var userSourceId = "63a816205d1af0e432fba6de";
                EntryDto.SourceId = userSourceId;
                EntryDto.UserId = user.Id;
                await ContentStore.EntryService.AddAsync(EntryDto, user.Id);
            }

            NavigationManager.NavigateTo("/");
        }
        #endregion
    }
}
