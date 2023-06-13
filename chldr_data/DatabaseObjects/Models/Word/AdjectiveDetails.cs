using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    // Прилагательное
    public class AdjectiveDetails : IWordDetails
    {
        // качественные и относительные
        public AdjectiveSemanticType SemanticType { get; set; }
        // зависимые и независимые
        public AdjectiveCharacteristic Characteristic { get; set; }
        public Case Case { get; set; }
        // Степень - только у качественных
        public Degree? Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        public int? Class { get; set; }
    }
}
