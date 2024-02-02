using core.Enums.WordDetails;
using core.Interfaces;

namespace core.DatabaseObjects.Models.Words
{
    public class PronounDetails : IDetails
    {
        public List<int> Classes { get; } = new List<int>() { 0,0,0};
        public Case Case { get; set; }
        public int Person { get; set; }
    }
}
