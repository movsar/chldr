using domain.Interfaces;
using domain.Enums.WordDetails;

namespace domain.DatabaseObjects.Models.Words
{
    public class NumeralDetails : IDetails
    {
        public NumericalComplexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case CaseSingular { get; set; }
        public Case CasePlural { get; set; }
    }
}
