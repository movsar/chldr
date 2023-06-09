using chldr_data.DatabaseObjects.Models;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataProvider
    {
        event Action? DatabaseInitialized;
        event Action<EntryModel>? EntryUpdated;
        event Action<EntryModel>? EntryInserted;
        event Action<EntryModel>? EntryDeleted;
        event Action<EntryModel>? EntryAdded;
        bool IsInitialized { get; set; }
        void Initialize();
        IUnitOfWork CreateUnitOfWork(string? userId = null);
        void PurgeAllData();
    }
}
