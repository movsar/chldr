using chldr_data.Enums.WordDetails;
using chldr_data.Interfaces;

namespace chldr_data.DatabaseObjects.Models.Words
{
    public class PronounDetails : IDetails
    {
        public List<int> Classes { get; } = new List<int>();
        public Case Case { get; set; }
        public int Person { get; set; }
    }
}
