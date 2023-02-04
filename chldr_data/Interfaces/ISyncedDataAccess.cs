using chldr_data.Services;
using Realms.Sync;

namespace chldr_data.Interfaces
{
    public interface ISyncedDataAccess : IDataAccess
    {
        App App { get; }

        event Action DatabaseSynchronized;
    }
}
