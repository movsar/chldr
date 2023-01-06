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
        public List<EntryModel> Entries { get; } = new List<EntryModel>();
        public Mode SearchMode { get; }
        public SearchResultsModel(Mode searchMode)
        {
            SearchMode = searchMode;
        }
    }
}
