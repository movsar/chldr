using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class VerbDetails : IWordDetails
    {
        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumeralType NumericalType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
    }
}
