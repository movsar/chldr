using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.Repositories;
using chldr_data.Services;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        event Action<DataSourceType> DatasourceInitialized;
        void SetActiveDataservice(DataSourceType dataSourceType);
        IDataSourceService GetActiveDataservice();
        void RemoveAllEntries();
        Repository GetRepository<T>() where T : IPersistentModelBase;
    }
}