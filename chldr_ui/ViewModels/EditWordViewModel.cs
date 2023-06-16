using chldr_data.Enums;
using chldr_data.DatabaseObjects.Dtos;
using chldr_shared.Stores;
using chldr_shared.Validators;
using chldr_data.DatabaseObjects.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums.WordDetails;

namespace chldr_ui.ViewModels
{
    public class EditWordViewModel : EditFormViewModelBase<EntryDto, EntryValidator>
    {
        [Parameter] public string? EntryId { get; set; }
        public EntryDto EntryDto { get; set; } = new EntryDto();
        protected string SourceId { get; set; } = "63a816205d1af0e432fba6de";

        #region WordDetails
        public VerbDetails VerbDetails { get; set; } = new VerbDetails();
        public NounDetails NounDetails { get; set; } = new NounDetails();
        public ConjunctionDetails ConjunctionDetails { get; set; } = new ConjunctionDetails();
        public PronounDetails PronounDetails { get; set; } = new PronounDetails();
        public AdverbDetails AdverbDetails { get; set; } = new AdverbDetails();
        public AdjectiveDetails AdjectiveDetails { get; set; } = new AdjectiveDetails();
        public NumeralDetails NumeralDetails { get; set; } = new NumeralDetails();
        public ParticleDetails ParticleDetails { get; set; } = new ParticleDetails();
        public MasdarDetails MasdarDetails { get; set; } = new MasdarDetails();
        public InterjectionDetails InterjectionDetails { get; set; } = new InterjectionDetails();
        public GerundDetails GerundDetails { get; set; } = new GerundDetails();
        private void DeserializeWordDetails()
        {
            if (!string.IsNullOrEmpty(EntryDto.Details))
            {
                switch ((WordType)EntryDto.EntrySubtype)
                {
                    case WordType.Noun:
                        NounDetails = JsonConvert.DeserializeObject<NounDetails>(EntryDto.Details);
                        break;

                    case WordType.Verb:
                        VerbDetails = JsonConvert.DeserializeObject<VerbDetails>(EntryDto.Details);
                        break;

                    case WordType.Adjective:
                        AdjectiveDetails = JsonConvert.DeserializeObject<AdjectiveDetails>(EntryDto.Details);
                        break;

                    case WordType.Interjection:
                        InterjectionDetails = JsonConvert.DeserializeObject<InterjectionDetails>(EntryDto.Details);
                        break;

                    case WordType.Conjunction:
                        ConjunctionDetails = JsonConvert.DeserializeObject<ConjunctionDetails>(EntryDto.Details);
                        break;

                    case WordType.Adverb:
                        AdverbDetails = JsonConvert.DeserializeObject<AdverbDetails>(EntryDto.Details);
                        break;

                    case WordType.Gerund:
                        GerundDetails = JsonConvert.DeserializeObject<GerundDetails>(EntryDto.Details);
                        break;

                    case WordType.Masdar:
                        MasdarDetails = JsonConvert.DeserializeObject<MasdarDetails>(EntryDto.Details);
                        break;

                    case WordType.Numeral:
                        NumeralDetails = JsonConvert.DeserializeObject<NumeralDetails>(EntryDto.Details);
                        break;

                    case WordType.Pronoun:
                        PronounDetails = JsonConvert.DeserializeObject<PronounDetails>(EntryDto.Details);
                        break;

                    case WordType.Particle:
                        ParticleDetails = JsonConvert.DeserializeObject<ParticleDetails>(EntryDto.Details);
                        break;

                    default:
                        break;
                }
            }
        }
        private void SerializeWordDetails()
        {
            switch ((WordType)EntryDto.EntrySubtype)
            {
                case WordType.Noun:
                    EntryDto.Details = JsonConvert.SerializeObject(NounDetails);
                    break;

                case WordType.Verb:
                    EntryDto.Details = JsonConvert.SerializeObject(VerbDetails);
                    break;

                case WordType.Adjective:
                    EntryDto.Details = JsonConvert.SerializeObject(AdjectiveDetails);
                    break;

                case WordType.Interjection:
                    EntryDto.Details = JsonConvert.SerializeObject(InterjectionDetails);
                    break;

                case WordType.Conjunction:
                    EntryDto.Details = JsonConvert.SerializeObject(ConjunctionDetails);
                    break;

                case WordType.Adverb:
                    EntryDto.Details = JsonConvert.SerializeObject(AdverbDetails);
                    break;

                case WordType.Gerund:
                    EntryDto.Details = JsonConvert.SerializeObject(GerundDetails);
                    break;

                case WordType.Masdar:
                    EntryDto.Details = JsonConvert.SerializeObject(MasdarDetails);
                    break;

                case WordType.Numeral:
                    EntryDto.Details = JsonConvert.SerializeObject(NumeralDetails);
                    break;

                case WordType.Pronoun:
                    EntryDto.Details = JsonConvert.SerializeObject(PronounDetails);
                    break;

                case WordType.Particle:
                    EntryDto.Details = JsonConvert.SerializeObject(ParticleDetails);
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Lifecycle Event Handlers
        private bool isInitialized = false;
        protected override void OnInitialized()
        {
            if (!isInitialized)
            {
                if (string.IsNullOrEmpty(EntryId))
                {
                    return;
                }
                isInitialized = true;

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
                DeserializeWordDetails();
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
            SerializeWordDetails();

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
