using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Search;
using chldr_shared.Models;
using chldr_utils;
using chldr_utils.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Realms;
using Realms.Logging;
using Realms.Sync;
using System.Linq;
using System.Net.NetworkInformation;
using Entry = chldr_data.Entities.Entry;
using LogLevel = Realms.Logging.LogLevel;
namespace chldr_data.Services
{
    public class DataAccess : IDataAccess
    {
        #region Properties
        public Realms.Sync.App App => _realmService.GetApp();
        public Realm Database
        {
            get
            {
                var realm = _realmService.GetDatabase();
                if (App.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
                {
                    // Don't try to sync anything if a legitimate user hasn't logged in
                    //realm.SyncSession.Stop();
                }

                return realm;
            }
        }

        #endregion

        #region Events
        public event Action ConnectionInitialized;
        public event Action DatabaseInitialized;
        public event Action DatabaseSynchronized;
        public event Action<SearchResultModel> GotNewSearchResult;
        #endregion

        #region Fields
        public const int ResultsLimit = 100;
        public const int RandomEntriesLimit = 30;
        private ExceptionHandler _exceptionHandler;
        private readonly FileService _fileService;
        private readonly NetworkService _networkService;
        private readonly MainSearchEngine _searchEngine;
        private RealmService _realmService;
        #endregion

        #region Database Initialization and State Changes
        private async Task InitializeDatabase()
        {
            var language = Database.All<Language>().FirstOrDefault();
            if (language == null)
            {
                // TODO: What if there's no offline file and no network?
                await Database.SyncSession.WaitForDownloadAsync();
            }

            DatabaseInitialized?.Invoke();
        }

        private async Task SynchronizeDatabase()
        {
            await Database.SyncSession.WaitForDownloadAsync();
            await Database.SyncSession.WaitForUploadAsync();
            DatabaseSynchronized?.Invoke();
        }

        public async Task Initialize()
        {
            // Database.SyncSession.ConnectionState == ConnectionState.Disconnected

            try
            {
                await _realmService.InitializeApp();
                _realmService.InitialzieConfig(App.CurrentUser);
                ConnectionInitialized?.Invoke();

                new Task(async () => await InitializeDatabase()).Start();
                new Task(async () => await SynchronizeDatabase()).Start();

                await DatabaseMaintenance();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("998"))
                {
                    // Network error
                    _exceptionHandler.ProcessError(new Exception("NETWORK_ERROR"));
                }
                else
                {
                    _exceptionHandler.ProcessError(ex);
                }
            }
        }
        #endregion

        public UserModel GetCurrentUserInfo()
        {
            if (!_networkService.IsNetworUp || Database.SyncSession.State == SessionState.Inactive)
            {
                throw new Exception(AppConstants.DataErrorMessages.NetworkIsDown);
            }

            if (App?.CurrentUser?.Id == null)
            {
                throw new Exception(AppConstants.DataErrorMessages.AppNotInitialized);
            }

            if (App.CurrentUser.Provider == Credentials.AuthProvider.Anonymous)
            {
                throw new Exception(AppConstants.DataErrorMessages.AnonymousUser);
            }

            var appUserId = new ObjectId(App.CurrentUser.Id);
            var user = Database.All<Entities.User>().FirstOrDefault(u => u._id == appUserId);

            if (user == null)
            {
                throw new Exception(AppConstants.DataErrorMessages.NoUserInfo);
            }

            return new UserModel(user);
        }

        public async Task FindAsync(string inputText)
        {
            await _searchEngine.FindAsync(inputText);
        }

        /*
        TEST

        1. That it executes without errors
        2. That it returns {RandomEntriesLimit} amount of Entries
         */
        public void LoadRandomEntries()
        {
            var randomizer = new Random();

            using var realm = Database;

            // Takes random entries and shuffles them to break the natural order
            var entries = realm.All<Entry>().AsEnumerable()
                .Where(entry => entry.Rate > 0)
                .OrderBy(x => randomizer.Next(0, 70000))
                .Take(RandomEntriesLimit)
                .OrderBy(entry => entry.GetHashCode())
                .Select(entry => EntryModelFactory.CreateEntryModel(entry))
                .ToList();

            var args = new SearchResultModel(entries);
            OnNewResults(args);
        }



        public DataAccess(FileService fileService, RealmService realmService, ExceptionHandler exceptionHandler, NetworkService networkService)
        {
            _exceptionHandler = exceptionHandler;
            _fileService = fileService;
            _fileService.PrepareDatabase();
            _networkService = networkService;
            _searchEngine = new MainSearchEngine(this);
            _realmService = realmService;

            if (App != null)
            {
                return;
            }

            // Runs asynchronously, then continues
            new Task(async () => await Initialize()).Start();
        }

        private async Task DatabaseMaintenance()
        {
            // Whatever is needed to be done with the database
        }

        public async Task LogInEmailPasswordAsync(string email, string password)
        {
            // Don't touch this unless it's absolutely necessary! It was very hard to configure!
            var appUser = await App.LogInAsync(Credentials.EmailPassword(email, password));
            _realmService.InitialzieConfig(appUser);
        }

        public async Task LogOutAsync()
        {
            var anonymousUser = App.AllUsers.FirstOrDefault(u => u.Provider == Credentials.AuthProvider.Anonymous);
            if (anonymousUser == null)
            {
                throw new Exception("not good");
            }

            App.SwitchUser(anonymousUser);
            _realmService.InitialzieConfig(anonymousUser);
        }

        public void OnNewResults(SearchResultModel results)
        {
            GotNewSearchResult?.Invoke(results);
        }

        public WordModel GetWordById(ObjectId entityId)
        {
            return new WordModel(_realmService.GetDatabase().All<Word>().FirstOrDefault(w => w._id == entityId));
        }

        public PhraseModel GetPhraseById(ObjectId entityId)
        {
            return new PhraseModel(_realmService.GetDatabase().All<Phrase>().FirstOrDefault(p => p._id == entityId));
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

        public EntryModel GetEntryById(ObjectId entryId)
        {
            var entry = Database.All<Entry>().FirstOrDefault(e => e._id == entryId);
            if (entry == null)
            {
                throw new Exception("Error:Requested_entry_couldn't_be_found");
            }
            return EntryModelFactory.CreateEntryModel(entry);
        }

        public PhraseModel GetPhraseByEntryId(ObjectId entryId)
        {
            return GetEntryById(entryId) as PhraseModel;
        }
        public WordModel GetWordByEntryId(ObjectId entryId)
        {
            return GetEntryById(entryId) as WordModel;
        }
        public List<LanguageModel> GetAllLanguages()
        {
            try
            {
                var languages = Database.All<Language>().AsEnumerable().Select(l => new LanguageModel(l));
                return languages.ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<SourceModel> GetAllNamedSources()
        {
            return Database.All<Source>().AsEnumerable().Select(s => new SourceModel(s)).ToList();
        }

    }

}
