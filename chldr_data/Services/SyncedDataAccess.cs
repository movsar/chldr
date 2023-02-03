using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Search;
using chldr_shared.Models;
using chldr_utils;
using chldr_utils.Models;
using chldr_utils.Services;
using Realms;
using Realms.Sync;
using System.Diagnostics;
using Entry = chldr_data.Entities.Entry;
using LogLevel = Realms.Logging.LogLevel;
namespace chldr_data.Services
{
    public class SyncedDataAccess : DataAccess, ISyncedDataAccess
    {
        #region Properties
        public App App => _realmService.GetApp();
        public UserService UserService { get; }

        public override Realm Database => _realmService.GetDatabase();
        #endregion

        #region Events
        public event Action ConnectionInitialized;
        public event Action DatabaseSynchronized;
        #endregion

        #region Fields

        #endregion

        public SyncedDataAccess(SyncedRealmService realmService, ExceptionHandler exceptionHandler, NetworkService networkService, UserService userService) : base(realmService, exceptionHandler, networkService)
        {
            if (App != null)
            {
                return;
            }

            UserService = userService;

            // Runs asynchronously, then continues
            new Task(async () => await Initialize()).Start();
        }

        #region DB Initializaion Related
        private async Task InitializeDatabase()
        {
            try
            {
                var language = Database.All<Language>().FirstOrDefault();
                if (language == null)
                {
                    // TODO: What if there's no offline file and no network?
                    await Database.SyncSession.WaitForDownloadAsync();
                }

                OnDatabaseInitialized();
            }
            catch (Exception)
            {
                Debug.WriteLine("ERR");
                throw;
            }

        }

        private async Task SynchronizeDatabase()
        {
            await Database.SyncSession.WaitForDownloadAsync();
            await Database.SyncSession.WaitForUploadAsync();
            DatabaseSynchronized?.Invoke();
        }

        public override async Task Initialize()
        {
            // Database.SyncSession.ConnectionState == ConnectionState.Disconnected

            try
            {
                await _realmService.InitializeApp();
                _realmService.InitializeConnection();
                ConnectionInitialized?.Invoke();

                new Task(async () =>
                {
                    await InitializeDatabase();
                    await SynchronizeDatabase();
                    await DatabaseMaintenance();
                }).Start();
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

        private async Task DatabaseMaintenance()
        {
            // Whatever is needed to be done with the database
        }

        #endregion



        public override async Task FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            await _searchEngine.FindAsync(inputText, filtrationFlags);
        }

        /*
        TEST

        1. That it executes without errors
        2. That it returns {RandomEntriesLimit} amount of Entries
         */
    }
}
