using chldr_data.Entities;
using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.Words
{
    public class VerbModel : WordModel
    {
        public VerbModel(Entry entry) : base(entry)
        {
        }

        public Dictionary<VerbTenses, List<VerbConjugation>> Tenses { get; } = new();
        public VerbMood Moods { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public YesNoUnset Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
    }
}
