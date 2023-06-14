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
                }

                EntryDto = EntryDto.FromModel(existingEntry);
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
                            ParticleDetails= JsonConvert.DeserializeObject<ParticleDetails>(EntryDto.Details);
                            break;

                        default:
                            break;
                    }
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
