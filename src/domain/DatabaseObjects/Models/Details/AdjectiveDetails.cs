using domain.Interfaces;
using domain.Enums.WordDetails;

namespace domain.DatabaseObjects.Models.Words
{
    // Прилагательное
    public class AdjectiveDetails : IDetails
    {
        // качественные и относительные
        public AdjectiveSemanticType SemanticType { get; set; }
        // зависимые и независимые
        public AdjectiveCharacteristics Characteristic { get; set; }
        public Case SingularCase { get; set; }
        public Case PluralCase { get; set; }
        // Степень - только у качественных
        public Degree? Degree { get; set; }
        // Грамматический класс - только у качественных и то, некоторых 
        public int? Class { get; set; }
    }
}
