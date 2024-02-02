using core.DatabaseObjects.Dtos;
using core.DatabaseObjects.Models.Words;
using core.Enums;
using core.Enums.WordDetails;
using core.Interfaces;
using Newtonsoft.Json;

namespace core.Helpers
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
