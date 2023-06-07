using chldr_data.Enums;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataSourceService
    {
        event Action? LocalDatabaseInitialized;
        Realm GetDatabase();
        void InitializeDatabase();
        void RemoveAllEntries();
    }
}
