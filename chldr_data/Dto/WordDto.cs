using chldr_data.Enums.WordDetails;
using chldr_data.Models.Words;
using MongoDB.Bson;
using System.Security.Claims;

namespace chldr_data.Dto
{
    public class WordDto : EntryDto
    {

        public WordDto() { }
        public WordDto(WordModel word) : base(word)
        {
            WordId = word.WordId.ToString();
            Content = word.Content;
            Notes = word.Notes;
            PartOfSpeech = word.PartOfSpeech;

            switch (PartOfSpeech)
            {
                case PartOfSpeech.Undefined:
                    break;
                case PartOfSpeech.Verb:
                    var verb = word as VerbModel;
                    if (verb == null)
                    {
                        return;
                    }
                    Tense = verb.Tense;
                    Conjugation = verb.Conjugation;
                    Mood = verb.Mood;
                    NumericalType = verb.NumericalType;
                    Transitiveness = verb.Transitiveness;
                    Class = verb.Class;
                    break;
                case PartOfSpeech.Noun:
                    break;
                case PartOfSpeech.Adverb:
                    break;
                case PartOfSpeech.Adjective:
                    break;
                case PartOfSpeech.Numeral:
                    break;
                case PartOfSpeech.Pronoun:
                    break;
                case PartOfSpeech.Conjunction:
                    break;
                case PartOfSpeech.Particle:
                    break;
                case PartOfSpeech.Interjection:
                    break;
                case PartOfSpeech.Masdar:
                    break;
                case PartOfSpeech.Gerund:
                    break;
                default:
                    break;
            }
        }

        #region Main Details
        public string? WordId { get; }
        public string Content { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public PartOfSpeech PartOfSpeech { get; set; }
        #endregion

        #region Verb Details
        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
        #endregion


    }
}
