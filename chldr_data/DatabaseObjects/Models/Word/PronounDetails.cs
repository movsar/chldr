using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class PronounDetails : IDetails
    {
        public List<int> Classes { get; } = new List<int>() { 0, 0, 0 };
        public int Person { get; set; }
    }
}
