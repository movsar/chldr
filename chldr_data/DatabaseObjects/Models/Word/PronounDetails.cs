using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class PronounDetails : IWordDetails
    {

        // Class can be 1,2,3,4,5 or 6
        public List<int> Classes { get; } = new List<int>() { 0, 0, 0 };

        // Person can be 1,2 or 3
        public int Person { get; }
    }
}
