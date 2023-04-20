using chldr_data.Entities;
using chldr_data.Enums.WordDetails;
using chldr_data.Models.Words;

namespace chldr_data.Models.Entries
{
    public class NumeralModel : WordModel
    {
        public NumeralModel(RealmEntry entry) : base(entry)
        {
        }

        public Complexity Complexity { get; set; }
        public NumericalCategory Category { get; set; }
        public Case Case { get; set; }
    }
}
