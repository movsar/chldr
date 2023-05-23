using chldr_data.Enums;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataSourceService
    {
        event Action? DatasourceInitialized;
        Realm GetDatabase();
        void InitializeConfiguration();
        void RemoveAllEntries();
    }
}
