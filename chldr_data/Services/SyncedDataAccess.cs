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

        public SyncedDataAccess(IRealmServiceFactory realmServiceFactory, UserService userService, ExceptionHandler exceptionHandler, NetworkService networkService) : base(realmServiceFactory, exceptionHandler, networkService)
        {
            _realmService = (realmServiceFactory.GetInstance(DataAccessType.Synced) as SyncedRealmService)!;
        }

        public App App => _realmService.GetApp();

        public override Realm Database => _realmService.GetDatabase();
        #endregion

        #region Events
        public event Action DatabaseSynchronized;
        #endregion


        #region DB Initializaion Related

        private async Task DatabaseMaintenance()
        {
            // Whatever is needed to be done with the database
        }

        #endregion
    }
}
