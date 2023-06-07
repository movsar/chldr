using Realms;

namespace chldr_data.Interfaces
{
    public interface ILocalDbReader
    {
        event Action DataSourceInitialized;
        void RemoveAllEntries();
        void InitializeDataSource();
    }
}