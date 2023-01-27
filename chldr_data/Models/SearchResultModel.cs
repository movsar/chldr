using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public IEnumerable<EntryModel> Entries { get; }
        public Mode SearchMode { get; }

        public SearchResultModel(IEnumerable<EntryModel> resultingEntries)
        {
            Entries = resultingEntries;
        }
        public SearchResultModel(string inputText, IEnumerable<EntryModel> resultingEntries, Mode mode)
        {
            SearchQuery = inputText;
            Entries = resultingEntries;
            SearchMode = mode;
        }
    }
}
