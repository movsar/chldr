using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Interfaces
{
    public enum DataAccessType
    {
        Synced,
        Offline
    }
    public interface IDataAccessFactory
    {
        IDataAccess GetInstance(DataAccessType dataAccessType);
    }
}
