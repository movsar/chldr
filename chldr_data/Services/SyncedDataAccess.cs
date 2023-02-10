using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms.Sync;

namespace chldr_data.Services
{
    public class SyncedDataAccess : DataAccess, ISyncedDataAccess
    {
        public event Action? DatabaseSynchronized;
        public App App => ((SyncedRealmService)_realmServiceFactory.GetInstance(DataAccessType.Synced)).GetApp();
        public SyncedDataAccess(IRealmServiceFactory realmServiceFactory, ExceptionHandler exceptionHandler, NetworkService networkService)
            : base(realmServiceFactory, exceptionHandler, networkService) { }
    }
}
