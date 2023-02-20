using chldr_data.Entities;
using chldr_data.Enums.WordDetails;

namespace chldr_data.Models.Entries
{
    public class NumeralModel : WordModel
    {
        public NumeralModel(Entry entry) : base(entry)
        {
        }

        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case Cases { get; set; }
    }
}
