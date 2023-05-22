using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces.DatabaseEntities;

namespace chldr_data.Models.Words
{
    public class WordDetailsModel : IWordDetails
    {
        public WordDetailsModel()
        {
            // Empty constructor
        }
        public WordDetailsModel(IWord word, PartOfSpeech partOfSpeech)
        {
            switch (partOfSpeech)
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
                    Classes[0] = verb.Class;
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

        // Class can be 1,2,3,4,5 or 6
        // Should be available only for some verbs, and nouns so don't allow to set it for all verbs
        // Person can have 1,2 or 3 classes
        public int[] Classes { get; set; } = new int[] { 0, 0, 0 };

        #region Pronoun Details
        // Person can be 1,2 or 3
        public int Person { get; set; }
        #endregion

        #region Verb Details
        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        #endregion

        #region Adjective Details
        // качественные и относительные
        public AdjectiveSemanticType SemanticType { get; set; }
        // зависимые и независимые
        public AdjectiveCharacteristic Characteristic { get; set; }
        public Case Case { get; set; }
        // Степень - только у качественных
        public Degree? Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        #endregion

        #region Numeral Details
        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        #endregion


    }
}
