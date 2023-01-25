using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Search;
using MongoDB.Bson;
using Realms;
using Realms.Sync;
using System.Linq;
using Entry = chldr_data.Entities.Entry;

namespace chldr_data.Services
{
    public class DataAccess : IDataAccess
    {
        #region Properties
        public Realms.Sync.App App => _realmService.GetApp();
        public Realm Database => _realmService.GetRealm();
        #endregion

        #region Events
        public event Action DatabaseInitialized;
        public event Action<SearchResultsModel> GotResults;
        #endregion

        #region Fields
        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;
        private RealmService _realmService;
        #endregion

        public async Task<UserModel?> GetCurrentUserInfoAsync()
        {
            // await App.CurrentUser.LogOutAsync();
            if (App.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
            {
                return null;
            }

            var appUserId = new ObjectId(App.CurrentUser.Id);
            await Database.Subscriptions.WaitForSynchronizationAsync();

            var user = Database.All<Entities.User>().First(u => u._id == appUserId);
            return new UserModel(user);
        }

        public async Task FindAsync(string inputText)
        {
            var searchEngine = new MainSearchEngine(this);
            await searchEngine.FindAsync(inputText);
        }
        public IEnumerable<EntryModel> GetRandomEntries()
        {
            var randomizer = new Random();
            var entries = Database.All<Entry>().AsEnumerable();

            // Takes random entries and shuffles them to break the natural order
            return entries
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => EntryModelFactory.CreateEntryModel(entry));
        }

        public DataAccess(FileService fileService, RealmService realmService)
        {
            _realmService = realmService;

            if (App != null)
            {
                return;
            }

            fileService.PrepareDatabase();

            _realmService.DatabaseInitialized += () =>
            {
                DatabaseInitialized?.Invoke();
            };

            Task.Run(async () => await _realmService.Initialize());
        }

        public void OnNewResults(SearchResultsModel results)
        {
            GotResults?.Invoke(results);
        }

        public WordModel GetWordById(ObjectId entityId)
        {
            return new WordModel(_realmService.GetRealm().All<Word>().FirstOrDefault(w => w._id == entityId));
        }

        public PhraseModel GetPhraseById(ObjectId entityId)
        {
            return new PhraseModel(_realmService.GetRealm().All<Phrase>().FirstOrDefault(p => p._id == entityId));
        }
        public async Task RegisterNewUserAsync(string email, string password)
        {
            await App.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public async Task SendPasswordResetRequestAsync(string email)
        {
            await App.EmailPasswordAuth.CallResetPasswordFunctionAsync(email, "somerandomsuperhardpassword");
        }

        public async Task UpdatePasswordAsync(string token, string tokenId, string newPassword)
        {
            await App.EmailPasswordAuth.ResetPasswordAsync(newPassword, token, tokenId);
        }

        public async Task ConfirmUserAsync(string token, string tokenId, string userEmail)
        {
            await App.EmailPasswordAuth.ConfirmUserAsync(token, tokenId);
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            await App.LogInAsync(Credentials.EmailPassword(email, password));
        }

        public async Task LogOutAsync()
        {
            await App.CurrentUser.LogOutAsync();
        }
    }

}
