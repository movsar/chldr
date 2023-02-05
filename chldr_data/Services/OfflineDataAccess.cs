using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using Realms;

namespace chldr_data.Services
{
    public class OfflineDataAccess : DataAccess
    {
        private new OfflineRealmService _realmService;

        public OfflineDataAccess(IRealmServiceFactory realmServiceFactory, ExceptionHandler exceptionHandler, NetworkService networkService) : base(realmServiceFactory, exceptionHandler, networkService)
        {
            _realmService = (realmServiceFactory.GetInstance(DataAccessType.Offline) as OfflineRealmService)!;
        }

        public override Realm Database => _realmService.GetDatabase();

        #region DB Initializaion Related


        #endregion
    }
}
