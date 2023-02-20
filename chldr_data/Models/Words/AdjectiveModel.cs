using chldr_data.Entities;
using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.Entries
{
    public class AdjectiveModel : WordModel
    {
        public AdjectiveModel(Entry entry) : base(entry)
        {
        }

        // качественные и относительные
        public AdjectiveSemanticType SemanticType { get; set; }
        // зависимые и независимые
        public AdjectiveCharacteristic Characteristic { get; set; }

        // 2 склонения для качественных (зависимые и независимые), 2 склонения для относительных (зависимые и независимые) см. Мациев
        public Case QualitiveDependentDeclensionCases { get; set; }
        public Case QualitiveIndependentDeclensionCases { get; set; }
        public Case RelativeDependentDeclensionCases { get; set; }
        public Case RelativeIndependentDeclensionCases { get; set; }


        // Степень - только у качественных
        public Degree Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        public int Class { get; set; }
    }
}
