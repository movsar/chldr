using chldr_data.DatabaseObjects.Models;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.remote.Services
{
    public class SqlSearchService : ISearchService
    {
        public event Action<SearchResultModel>? GotNewSearchResult;

        public List<EntryModel> Find(string inputText, int limit)
        {
            throw new NotImplementedException();
        }

        public Task FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            throw new NotImplementedException();
        }

        public List<EntryModel> GetEntriesOnModeration()
        {
            throw new NotImplementedException();
        }

        public List<EntryModel> GetLatestEntries()
        {
            throw new NotImplementedException();
        }

        public List<EntryModel> GetWordsToFiddleWith()
        {
            throw new NotImplementedException();
        }
    }
}
