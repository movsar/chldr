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
            Initialize();
        }

        #region DB Initializaion Related

        public override void Initialize()
        {
            // Database.SyncSession.ConnectionState == ConnectionState.Disconnected

            try
            {
                _realmService.InitializeConfiguration();
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
