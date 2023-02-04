using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;

namespace chldr_data.Services
{
    public class OfflineDataAccess : DataAccess
    {
        public OfflineDataAccess(IRealmServiceFactory realmServiceFactory, ExceptionHandler exceptionHandler, NetworkService networkService) : base(realmServiceFactory.GetInstance(DataAccessType.Offline), exceptionHandler, networkService)
        { }
        #region Properties
        public override Realm Database => _realmService.GetDatabase();
        #endregion

        #region DB Initializaion Related

        public override void Initialize()
        {
            // Database.SyncSession.ConnectionState == ConnectionState.Disconnected

            try
            {
                _realmService.InitializeConfiguration();
                OnDatabaseInitialized();
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
