using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class NumeralDetails : IWordDetails
    {
        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case Case { get; set; }
    }
}
