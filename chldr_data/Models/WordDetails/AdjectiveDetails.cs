using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.WordDetails
{
    public class AdjectiveDetails
    {
        // 4 склонения - см. Мациев
        public Declension Declension { get; set; }
        public AdjectiveSemanticType SemanticType { get; set; }
        public AdjectiveCharacteristic Characteristic { get; set; }

        // Only if Semantic Type is Qualitive
        public Degree Degree { get; set; }
    }
}
