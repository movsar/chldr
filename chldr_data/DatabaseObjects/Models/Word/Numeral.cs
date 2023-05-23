using chldr_data.Enums.WordDetails;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class Numeral : WordModel
    {
        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case Case { get; set; }
    }
}
