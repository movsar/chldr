using chldr_data.Enums;
using chldr_data.Models;
using chldr_data.DatabaseObjects.Interfaces;
using chldr_data.Repositories;
using chldr_data.Services;
using Realms;

namespace chldr_data.Interfaces
{
    public interface IDataAccess
    {
        event Action DatasourceInitialized;
        Realm GetDatabase();
        void RemoveAllEntries();
        Repository GetRepository<T>() where T : IEntity;
        IGraphQLRequestSender RequestSender { get; set; }

    }
}