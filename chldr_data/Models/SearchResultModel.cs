using System.Collections.ObjectModel;
using chldr_data.DatabaseObjects.Models;

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
        public Mode SearchMode { get; }
        public ObservableCollection<EntryModel> Entries { get; } = new();

        public SearchResultModel(List<EntryModel> resultingEntries)
        {
            Entries.Clear();
            foreach (var entry in resultingEntries)
            {
                Entries.Add(entry);
            }
        }
        public SearchResultModel(string inputText, List<EntryModel> resultingEntries, Mode mode)
        {
            SearchQuery = inputText;
            SearchMode = mode;

            Entries.Clear();
            foreach (var entry in resultingEntries)
            {
                Entries.Add(entry);
            }
        }
    }
}
