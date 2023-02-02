using chldr_data.Services;
using Realms.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public interface ISyncedDataAccess : IDataAccess
    {
        App App { get; }

        event Action ConnectionInitialized;
        event Action DatabaseSynchronized;

        UserService UserService { get; }
    }
}
