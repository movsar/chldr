using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.WordDetails
{
    public class VerbDetails
    {
        public Dictionary<VerbTenses, List<VerbConjugation>> Tenses { get; } = new();
        public VerbMoods Moods { get; set; } = new();
        public NumericalType NumericalType { get; set; }
        public YesNoUnset Transitiveness { get; set; }
        // Should be available only for some verbs, so don't allow to set it for all verbs
        public int Class { get; set; } = 0;
    }
}
