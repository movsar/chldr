using domain.Interfaces;
using domain.Enums.WordDetails;

namespace domain.DatabaseObjects.Models.Words
{
    public class PronounDetails : IDetails
    {
        public List<int> Classes { get; } = new List<int>() { 0,0,0};
        public Case Case { get; set; }
        public int Person { get; set; }
    }
}
