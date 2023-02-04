using Realms;

namespace chldr_data.Interfaces
{
    public interface IRealmService
    {
        event Action? DatasourceInitialized;
        Realm GetDatabase();
        void InitializeConfiguration();
        void InitializeDataSource();
    }
}
