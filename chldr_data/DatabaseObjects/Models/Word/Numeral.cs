using chldr_data.Enums.WordDetails;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class Numeral : EntryModel
    {
        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case Case { get; set; }
    }
}
