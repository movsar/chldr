using chldr_data.Enums;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IRealmService
    {
        event Action<DataSourceType>? DatasourceInitialized;
        Realm GetDatabase();
        void InitializeConfiguration();
        void Initialize();
    }
}
