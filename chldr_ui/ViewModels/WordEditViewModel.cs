using Blazored.Modal;
using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums.WordDetails;
using chldr_data.Helpers;
using chldr_data.Interfaces;
using chldr_shared;
using chldr_ui.Components;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : ViewModelBase
    {
        [Inject] IDataProvider DataProvider { get; set; }
        [Parameter]
        public EntryDto EntryDto { get; set; }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            EntryDto.Details = SerializeWordDetails((WordType)EntryDto.EntrySubtype);
            return base.OnAfterRenderAsync(firstRender);
        }
        protected override Task OnParametersSetAsync()
        {
            if (EntryDto.Details != "")
            {
                DeserializeWordDetails();
            }

            if (!string.IsNullOrEmpty(EntryDto.ParentEntryId))
            {

                var unitOfWork = DataProvider.CreateUnitOfWork();
                ParentEntry = unitOfWork.Entries.Get(EntryDto.ParentEntryId);
            }

            return base.OnParametersSetAsync();
        }

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
        #endregion
        internal EntryModel? ParentEntry { get; set; }

        protected async Task OpenSetParentWordDialog()
        {


            var entrySelectionDialog = Modal.Show<WordSelectorDialog>("", new ModalOptions()
            {
                HideHeader = true,
                Size = ModalSize.Large,
                HideCloseButton = true
            });

            var result = await entrySelectionDialog.Result;
            if (result.Confirmed)
            {
                ParentEntry = result.Data as EntryModel;
                EntryDto.ParentEntryId = ParentEntry?.EntryId;
            }
        }
        private void DeserializeWordDetails()
        {
            if (string.IsNullOrEmpty(EntryDto.Details))
            {
                return;
            }

            var details = WordHelper.DeserializeWordDetails((WordType)EntryDto.EntrySubtype, EntryDto.Details);
            switch (details)
            {
                case VerbDetails verbDetails:
                    VerbDetails = verbDetails;
                    break;

                case NounDetails nounDetails:
                    NounDetails = nounDetails;
                    break;

                case ConjunctionDetails conjunctionDetails:
                    ConjunctionDetails = conjunctionDetails;
                    break;

                case PronounDetails pronounDetails:
                    PronounDetails = pronounDetails;
                    break;

                case AdverbDetails adverbDetails:
                    AdverbDetails = adverbDetails;
                    break;

                case AdjectiveDetails adjectiveDetails:
                    AdjectiveDetails = adjectiveDetails;
                    break;

                case NumeralDetails numeralDetails:
                    NumeralDetails = numeralDetails;
                    break;

                case ParticleDetails particleDetails:
                    ParticleDetails = particleDetails;
                    break;

                case MasdarDetails masdarDetails:
                    MasdarDetails = masdarDetails;
                    break;

                case InterjectionDetails interjectionDetails:
                    InterjectionDetails = interjectionDetails;
                    break;

                case GerundDetails gerundDetails:
                    GerundDetails = gerundDetails;
                    break;

                default:
                    // Handle unknown type or provide an error message
                    break;
            }
        }
        private string SerializeWordDetails(WordType wordType)
        {
            switch (wordType)
            {
                case WordType.Noun:
                    return JsonConvert.SerializeObject(NounDetails);

                case WordType.Verb:
                    return JsonConvert.SerializeObject(VerbDetails);

                case WordType.Adjective:
                    return JsonConvert.SerializeObject(AdjectiveDetails);

                case WordType.Interjection:
                    return JsonConvert.SerializeObject(InterjectionDetails);

                case WordType.Conjunction:
                    return JsonConvert.SerializeObject(ConjunctionDetails);

                case WordType.Adverb:
                    return JsonConvert.SerializeObject(AdverbDetails);

                case WordType.Gerund:
                    return JsonConvert.SerializeObject(GerundDetails);

                case WordType.Masdar:
                    return JsonConvert.SerializeObject(MasdarDetails);

                case WordType.Numeral:
                    return JsonConvert.SerializeObject(NumeralDetails);

                case WordType.Pronoun:
                    return JsonConvert.SerializeObject(PronounDetails);

                case WordType.Particle:
                    return JsonConvert.SerializeObject(ParticleDetails);

                default:
                    return "";
            }
        }
    }
}
