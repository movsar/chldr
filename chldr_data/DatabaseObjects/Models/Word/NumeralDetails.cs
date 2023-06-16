using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class NumeralDetails : IDetails
    {
        public NumericalComplexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case CaseSingular { get; set; }
        public Case CasePlural { get; set; }
    }
}
