using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Services;

namespace chldr_data.Factories
{
    public class RealmServiceFactory : IRealmServiceFactory
    {
        public DataSourceType CurrentDataSource { get; set; } = DataSourceType.Offline;
        private readonly IEnumerable<IDataSourceService> _realmServiceImplementations;
        public RealmServiceFactory(IEnumerable<IDataSourceService> dataAccessImplementations)
        {
            _realmServiceImplementations = dataAccessImplementations;
        }
        private IDataSourceService GetService(Type type)
        {
            return _realmServiceImplementations.FirstOrDefault(x => x.GetType() == type)!;
        }

        public IDataSourceService GetInstance(DataSourceType realmServiceType)
        {
            switch (realmServiceType)
            {
                case DataSourceType.Synced:
                    var service = GetService(typeof(SyncedRealmService));
                    return service;
                case DataSourceType.Offline:
                    var service2 = GetService(typeof(OfflineRealmService));
                    return service2;
                default:
                    throw new Exception("Unknown data access type");
            }
        }

        public IDataSourceService GetActiveInstance()
        {
            switch (CurrentDataSource)
            {
                case DataSourceType.Synced:
                    return GetInstance(DataSourceType.Synced);
                case DataSourceType.Offline:
                    return GetInstance(DataSourceType.Offline);
                default:
                    throw new Exception("Unsupported realm service");
            }
        }
    }
}
