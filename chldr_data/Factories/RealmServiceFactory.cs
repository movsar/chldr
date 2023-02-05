using chldr_data.Interfaces;
using chldr_data.Services;

namespace chldr_data.Factories
{
    public class RealmServiceFactory : IRealmServiceFactory
    {
        private readonly IEnumerable<IRealmService> _realmServiceImplementations;
        public RealmServiceFactory(IEnumerable<IRealmService> dataAccessImplementations)
        {
            _realmServiceImplementations = dataAccessImplementations;
        }
        private IRealmService GetService(Type type)
        {
            return _realmServiceImplementations.FirstOrDefault(x => x.GetType() == type)!;
        }

        public IRealmService GetInstance(DataAccessType realmServiceType)
        {
            switch (realmServiceType)
            {
                case DataAccessType.Synced:
                    var service = GetService(typeof(SyncedRealmService));
                    return service;
                case DataAccessType.Offline:
                    var service2 = GetService(typeof(OfflineRealmService));
                    return service2;
                default:
                    throw new Exception("Unknown data access type");
            }
        }

        public IRealmService GetInstance()
        {
            switch (DataAccess.CurrentDataAccess)
            {
                case DataAccessType.Synced:
                    return GetInstance(DataAccessType.Synced);
                case DataAccessType.Offline:
                    return GetInstance(DataAccessType.Offline);
                default:
                    return null;
            }
        }
    }
}
