using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_data.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using chldr_data.Interfaces;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums.WordDetails;

namespace chldr_ui.ViewModels
{
    public class EditWordViewModel : EditFormViewModelBase<EntryDto, EntryValidator>
    {
        private bool isInitialized = false;
        [Parameter] public string? EntryId { get; set; }
        // Set "User" source id by default
        protected string SourceId { get; set; } = "63a816205d1af0e432fba6de";
        protected bool IsEditMode = false;
        private int _originalSubtype;
        public VerbDetails VerbDetails { get; set; } = new VerbDetails();
        public NounDetails NounDetails { get; set; } = new NounDetails();
        public EntryDto EntryDto { get; set; } = new EntryDto();
        protected override void OnInitialized()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                EntryDto.UserId = UserStore.ActiveSession.User!.UserId;
                EntryDto.SourceId = SourceId;

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
                    _originalSubtype = existingEntry.Subtype;
                }

                EntryDto = EntryDto.FromModel(existingEntry);
                if (!string.IsNullOrEmpty(EntryDto.Details))
                {
                    //WordDetails = JsonConvert.DeserializeObject<WordDetailsDto>(EntryDto.Details);
                }
            }
        }

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
        public async Task ValidateAndSubmitAsync()
        {
            await ValidateAndSubmitAsync(EntryDto, Save);
        }
        public async Task Save()
        {
            switch ((WordType)EntryDto.EntrySubtype)
            {
                case WordType.Noun:
                    EntryDto.Details = JsonConvert.SerializeObject(NounDetails);
                    break;
                case WordType.Verb:
                    EntryDto.Details = JsonConvert.SerializeObject(VerbDetails);
                    break;
                // TODO
                default:
                    break;
            }

            var user = UserModel.FromDto(UserStore.ActiveSession.User);
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
    }
}
