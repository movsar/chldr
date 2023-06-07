using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataProvider
    {
        event Action? LocalDatabaseInitialized;
        bool IsInitialized { get; set; }
        void Initialize();
        IUnitOfWork CreateUnitOfWork();
        void PurgeAllData();
    }
}
