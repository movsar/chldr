namespace chldr_data.Models.WordDetails
{
    public class PronounDetails
    {
        // Class can be 1,2,3,4,5 or 6
        public List<int> Classes { get; } = new List<int>();
        // Person can be 1,2 or 3
        public int Person { get; }
    }
}
