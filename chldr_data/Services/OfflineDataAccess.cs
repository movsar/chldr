using chldr_data.Entities;
using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;
using Realms.Sync;
using System.Diagnostics;

namespace chldr_data.Services
{
    public class OfflineDataAccess : DataAccess
    {
        #region Properties
        public override Realm Database => _realmService.GetDatabase();
        #endregion

        public OfflineDataAccess(IRealmService realmService, ExceptionHandler exceptionHandler, NetworkService networkService, UserService userService) : base(realmService, exceptionHandler, networkService)
        {
      
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

   
        public override async Task Initialize()
        {
            // Database.SyncSession.ConnectionState == ConnectionState.Disconnected

            try
            {
                _realmService.InitializeConfiguration();

                new Task(async () =>
                {
                    await InitializeDatabase();
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
        #endregion
    }
}
