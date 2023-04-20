using chldr_data.Entities;
using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.Words
{
    public class VerbModel : WordModel
    {
        public VerbModel(SqlEntry entry) : base(entry)
        {
        }

        public VerbTense Tense { get; set; }
        public VerbConjugation Conjugation { get; set; }
        public VerbMood Mood { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public Transitiveness Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
    }
}
