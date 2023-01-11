using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class SearchResultsModel
    {
        public enum Mode
        {
            Direct,
            Reverse,
            Random
        }

        public string InputText { get; }
        public List<EntryModel> Entries { get; }
        public Mode SearchMode { get; }

        public SearchResultsModel(string inputText, List<EntryModel> resultingEntries, Mode mode)
        {
            InputText = inputText;
            Entries = resultingEntries;
            SearchMode = mode;
        }
    }
}
