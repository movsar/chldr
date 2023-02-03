using chldr_data.Interfaces;
using chldr_data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Factories
{
    public class DataAccessFactory : IDataAccessFactory
    {
        private readonly IEnumerable<IDataAccess> _dataAccessImplementations;
        public DataAccessFactory(IEnumerable<IDataAccess> dataAccessImplementations)
        {
            _dataAccessImplementations = dataAccessImplementations;
        }
        private IDataAccess GetService(Type type)
        {
            return this._dataAccessImplementations.FirstOrDefault(x => x.GetType() == type)!;
        }

        public IDataAccess GetInstance(DataAccessType dataAccessType)
        {
            switch (dataAccessType)
            {
                case DataAccessType.Synced:
                    return GetService(typeof(SyncedDataAccess));
                case DataAccessType.Offline:
                    return GetService(typeof(OfflineDataAccess));
                default:
                    throw new Exception("Unknown data access type");
            }
        }
    }
}
