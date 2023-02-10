using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;

namespace chldr_data.Services
{
    public class OfflineDataAccess : DataAccess
    {
        public OfflineDataAccess(IRealmServiceFactory realmServiceFactory, ExceptionHandler exceptionHandler, NetworkService networkService)
            : base(realmServiceFactory, exceptionHandler, networkService) { }
    }
}
