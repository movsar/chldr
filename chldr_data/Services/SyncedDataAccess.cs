using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Sync;
using System.Diagnostics;

namespace chldr_data.Services
{
    public class SyncedDataAccess : DataAccess, ISyncedDataAccess
    {
        #region Properties
        private new SyncedRealmService _realmService;
        private readonly UserService _userService;

        public SyncedDataAccess(IRealmServiceFactory realmServiceFactory, UserService userService, ExceptionHandler exceptionHandler, NetworkService networkService) : base(realmServiceFactory.GetInstance(DataAccessType.Synced), exceptionHandler, networkService)
        {
            _userService = userService;
            _realmService = base._realmService as SyncedRealmService;
        }

        public App App => _realmService.GetApp();
        public UserService UserService { get; set; }

        public override Realm Database => _realmService.GetDatabase();
        #endregion

        #region Events
        public event Action ConnectionInitialized;
        public event Action DatabaseSynchronized;
        #endregion


        #region DB Initializaion Related
        private async Task InitializeDatabase()
        {
            try
            {
                var language = Database.All<Language>().FirstOrDefault();
                if (language == null)
                {
                    // TODO: What if there's no offline file and no network?
                    await SynchronizeDatabase();
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
            await Database.Subscriptions.WaitForSynchronizationAsync();
            await Database.SyncSession.WaitForDownloadAsync();
            await Database.SyncSession.WaitForUploadAsync();

            DatabaseSynchronized?.Invoke();
        }

        public override void Initialize()
        {
            if (App != null)
            {
                return;
            }

            UserService = _userService;

            // Runs asynchronously, then continues
            new Task(() => Initialize()).Start();

            // Database.SyncSession.ConnectionState == ConnectionState.Disconnected
            try
            {
                new Task(async () =>
                {
                    await _realmService.InitializeApp();
                    _realmService.InitializeConfiguration();
                    ConnectionInitialized?.Invoke();

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
    }
}
