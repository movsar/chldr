using domain.Interfaces;
using domain.Enums.WordDetails;

namespace domain.DatabaseObjects.Models.Words
{
    public class VerbDetails : IDetails
    {
        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumeralType NumeralType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
    }
}
