using chldr_data.Entities;
using chldr_data.Models.Words;

namespace chldr_data.Models.Entries
{
    public class PronounModel : WordModel
    {

        // Class can be 1,2,3,4,5 or 6
        public List<int> Classes { get; } = new List<int>() { 0, 0, 0 };

        // Person can be 1,2 or 3
        public int Person { get; }
    }
}
