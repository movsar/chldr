namespace chldr_data.Models
{
    public class SearchResultModel
    {
        public enum Mode
        {
            Direct,
            Reverse,
            Random
        }

        public string SearchQuery { get; }
        public List<EntryModel> Entries { get; }
        public Mode SearchMode { get; }

        public SearchResultModel(List<EntryModel> resultingEntries)
        {
            Entries = resultingEntries;
        }
        public SearchResultModel(string inputText, List<EntryModel> resultingEntries, Mode mode)
        {
            SearchQuery = inputText;
            Entries = resultingEntries;
            SearchMode = mode;
        }
    }
}
