using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Models.Words;

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
        public Case Case { get; set; }
        // Степень - только у качественных
        public Degree? Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        public int? Class { get; set; }
    }
}
