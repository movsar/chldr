using chldr_data.Entities;

namespace chldr_data.Models.Entries
{
    public class PronounModel : WordModel
    {
        public PronounModel(Entry entry) : base(entry)
        {
        }

        // Class can be 1,2,3,4,5 or 6
        public List<int> Classes { get; } = new List<int>();
        // Person can be 1,2 or 3
        public int Person { get; }
    }
}
