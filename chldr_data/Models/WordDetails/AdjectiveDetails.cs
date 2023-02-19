using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.WordDetails
{
    public class AdjectiveDetails
    {
        // качественные и относительные
        public AdjectiveSemanticType SemanticType { get; set; }
        // зависимые и независимые
        public AdjectiveCharacteristic Characteristic { get; set; }

        // 2 склонения для качественных (зависимые и независимые), 2 склонения для относительных (зависимые и независимые) см. Мациев
        public Cases QualitiveDependentDeclensionCases { get; set; }
        public Cases QualitiveIndependentDeclensionCases { get; set; }
        public Cases RelativeDependentDeclensionCases { get; set; }
        public Cases RelativeIndependentDeclensionCases { get; set; }


        // Степень - только у качественных
        public Degree Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        public int Class { get; set; }
    }
}
