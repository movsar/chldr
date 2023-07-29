using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models.Words;
using chldr_data.Enums;
using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;
using Newtonsoft.Json;

namespace chldr_data.Helpers
{
    public static class WordHelper
    {

        public static IDetails? DeserializeWordDetails(WordType wordType, string details)
        {
            if (string.IsNullOrEmpty(details))
            {
                return null;
            }

            switch (wordType)
            {
                case WordType.Noun:
                    return JsonConvert.DeserializeObject<NounDetails>(details);

                case WordType.Verb:
                    return JsonConvert.DeserializeObject<VerbDetails>(details);

                case WordType.Adjective:
                    return JsonConvert.DeserializeObject<AdjectiveDetails>(details);

                case WordType.Interjection:
                    return JsonConvert.DeserializeObject<InterjectionDetails>(details);

                case WordType.Conjunction:
                    return JsonConvert.DeserializeObject<ConjunctionDetails>(details);

                case WordType.Adverb:
                    return JsonConvert.DeserializeObject<AdverbDetails>(details);

                case WordType.Gerund:
                    return JsonConvert.DeserializeObject<GerundDetails>(details);

                case WordType.Masdar:
                    return JsonConvert.DeserializeObject<MasdarDetails>(details);

                case WordType.Numeral:
                    return JsonConvert.DeserializeObject<NumeralDetails>(details);

                case WordType.Pronoun:
                    return JsonConvert.DeserializeObject<PronounDetails>(details);

                case WordType.Particle:
                    return JsonConvert.DeserializeObject<ParticleDetails>(details);

                default:
                    return null;
            }
        }
    }
}
