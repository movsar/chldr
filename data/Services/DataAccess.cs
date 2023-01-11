using Data.Entities;
using Data.Models;
using Data.Search;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public abstract class DataAccess
    {
        public Action<SearchResultsModel> GotNewSearchResults;

        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;
        internal abstract Realm GetRealmInstance();
        public async Task FindAsync(string inputText)
        {
            var searchEngine = new MainSearchEngine(this);
            await searchEngine.FindAsync(inputText);
        }
        public IEnumerable<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = GetRealmInstance().All<EntryEntity>().AsEnumerable();

            // Takes random entries and shuffles them to break the natural order
            return entries
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => new EntryModel(entry));
        }

        public async void DoDangerousTheStuff()
        {

        }
    }
}
