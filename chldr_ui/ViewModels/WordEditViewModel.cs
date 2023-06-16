using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums.WordDetails;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_ui.ViewModels
{
    public class WordEditViewModel : ViewModelBase
    {
        [Parameter]
        public EntryDto EntryDto { get; set; }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            SerializeWordDetails();
            return base.OnAfterRenderAsync(firstRender);
        }
        protected override Task OnParametersSetAsync()
        {
            if (EntryDto.Details != "")
            {
                DeserializeWordDetails();
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
        internal void SerializeWordDetails()
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

    }
}
