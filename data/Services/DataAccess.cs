using Data.Entities;
using Data.Models;
using Data.Search;
using Data.Services.PartialMethods;
using Entry = Data.Entities.Entry;

namespace Data.Services
{
    public class DataAccess
    {
        public Action<SearchResultsModel> GotNewSearchResults;

        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;

        public async Task FindAsync(string inputText)
        {
            var searchEngine = new MainSearchEngine(this);
            await searchEngine.FindAsync(inputText);
        }
        public IEnumerable<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = RealmService.GetRealm().All<Entry>().AsEnumerable();

            // Takes random entries and shuffles them to break the natural order
            return entries
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => new EntryModel(entry));
        }

        public DataAccess()
        {
            var fs = new FileService();
            fs.PrepareDatabase();
            RealmService.DatabaseInitialized += RealmService_DatabaseInitialized;
            Task.Run(() => RealmService.Initialize());
        }

        private void RealmService_DatabaseInitialized()
        {
            // Load random entries
            var searchResults = new SearchResultsModel("", GetRandomEntries(), SearchResultsModel.Mode.Random);
            GotNewSearchResults?.Invoke(searchResults);
        }

        public async Task RegisterNewUser(string email, string password, string username, string firstName, string lastName)
        {
            await RealmService.GetApp().EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public async Task ConfirmUser(string token, string tokenId)
        {
            await RealmService.GetApp().EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
        }
    }
}
